using System.Numerics;
using dex.monitor.Business.Chains.Abis;
using dex.monitor.Business.Chains.Abis.Models;
using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.DataStores.MemoryStores.TokensStore;
using dex.monitor.Business.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Nethereum.Web3;

namespace dex.monitor.Business.Chains.Internals.Eth;

internal class EthProvider(
    IChainFactory factory,
    ITokensStore tokensStore,
    IOptions<EthSettings> options,
    ILogger<EthProvider> logger) : IChainProvider
{
    private readonly Web3 _client = new(options.Value.ApiUrl, logger);

    public virtual string Network => ChainConstants.Eth;

    public async Task<List<SwapOperation>> CheckSwaps(string blockNumber, CancellationToken ct = default)
    {
        var currentBlock = long.Parse(blockNumber);
        var filters = factory.GetFilterParsers(Network);
        var block = await _client.Eth.Blocks
            .GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(currentBlock));

        if (block == null) return [];

        var tasks = block.Transactions
            .Select(e => new
            {
                parser = filters.FirstOrDefault(f => f.Acceptable(e)),
                tx = e,
            })
            .Where(e => e.parser != null)
            .Select(async e => await e.parser.Process(this, e.tx, ct));

        var swaps = await Task.WhenAll(tasks);

        return swaps.SelectMany(e => e).ToList();
    }

    public async Task<NetworkToken> GetTokenInfo(string address, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address)) return null;
        var exists = await tokensStore.GetToken(Network, address);
        if (exists != null)
            return exists;

        var query = _client.Eth.ERC20.GetContractService(address);
        try
        {
            var newToken = new NetworkToken
            {
                Network = Network,
                Address = address,
                Decimals = await query.DecimalsQueryAsync(),
                Code = await query.SymbolQueryAsync(),
                Name = await query.NameQueryAsync(),
            };

            return newToken;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not get token info: {Chain} {Contract}", Network, address);
            return null;
        }
    }

    public async Task<List<SwapEvent>> GetSwapEvents(string txHash, CancellationToken ct)
    {
        var receipt = await _client.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
        var logs = receipt.DecodeAllEvents<SwapEvent>().Select(e => e.Event).ToList();
        return logs;
    }

    public async Task<List<PoolInfo>> GetLiquidityPools(
        LiquidityPoolsRequest request, CancellationToken ct = default)
    {
        var pools = new List<PoolInfo>();
        var factoryContract = _client.Eth.GetContract(PancakeSwap.FactoryAbi, request.FactoryAddress);
        var allPairsLengthFunction = factoryContract.GetFunction("allPairsLength");
        var pairCount = await allPairsLengthFunction.CallAsync<BigInteger>();

        for (var i = 0; i < 20; i++)
        {
            var pairAddress = await factoryContract.GetFunction("allPairs").CallAsync<string>([i]);
            var pair = _client.Eth.GetContract(PancakeSwap.PairAbi, pairAddress);

            var getReservesFunction = pair.GetFunction("getReserves");
            var token0Function = pair.GetFunction("token0");
            var token1Function = pair.GetFunction("token1");

            var reserves = await getReservesFunction.CallAsync<Reserves>();
            var tokenOneAddress = await token0Function.CallAsync<string>();
            var tokenTwoAddress = await token1Function.CallAsync<string>();

            var tokenOne = await GetTokenInfo(tokenOneAddress, ct);
            var tokenTwo = await GetTokenInfo(tokenTwoAddress, ct);

            pools.Add(new PoolInfo
            {
                PairAddress = pairAddress,
                Token0 = tokenOneAddress,
                Token1 = tokenTwoAddress,
                Reserve0 = Web3.Convert.FromWei(reserves.ReserveOne, tokenOne.Decimals),
                Reserve1 = Web3.Convert.FromWei(reserves.ReserveTwo, tokenTwo.Decimals),
                LastUpdated = DateTimeOffset.UtcNow,
            });
        }

        return pools;
    }

    public async Task<ChainSubscription> SubscribeLogs<T>(
        string wsUrl, Action<WrappedEvent<T>> action, CancellationToken ct = default)
        where T : IEventDTO, new()
    {
        var ws = new StreamingWebSocketClient(wsUrl);
        var subs = new EthLogsObservableSubscription(ws);

        var filterSwaps = Event<T>.GetEventABI().CreateFilterInput();
        var subscription = subs.GetSubscriptionDataResponsesAsObservable().Subscribe(e =>
        {
            var anEvent = Event<T>.DecodeEvent(e);
            if (anEvent.Event != null)
                action(new WrappedEvent<T>
                {
                    AnEvent = anEvent.Event,
                    BlockNumber = (long)e.BlockNumber.Value,
                    TxId = e.TransactionHash,
                    LogIndex = (int)e.LogIndex.Value,
                });
        });

        await ws.StartAsync();
        await subs.SubscribeAsync(filterSwaps);

        return new ChainSubscription
        {
            Key = $"{Network}::{typeof(T).Name}",
            Client = ws,
            Subscription = subscription,
        };
    }
}