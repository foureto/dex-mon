using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.DataStores.MemoryStores;
using dex.monitor.Business.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
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

    public async Task<TokenInfo> GetTokenInfo(string address, CancellationToken ct = default)
    {
        var exists = await tokensStore.GetToken(Network, address);
        if (exists != null)
            return exists;

        var query = _client.Eth.ERC20.GetContractService(address);
        var newToken = new TokenInfo
        {
            Network = Network,
            Address = address,
            Decimals = await query.DecimalsQueryAsync(),
            Code = await query.SymbolQueryAsync(),
            Name = await query.NameQueryAsync(),
        };

        await tokensStore.AddToken(newToken);

        return newToken;
    }

    public async Task<List<SwapEvent>> GetSwapEvents(string txHash, CancellationToken ct)
    {
        var receipt = await _client.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
        var logs = receipt.DecodeAllEvents<SwapEvent>().Select(e => e.Event).ToList();
        return logs;
    }
}