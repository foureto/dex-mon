using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.Domain;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace dex.monitor.Business.Chains;

public interface IChainProvider
{
    string Network { get; }
    Task<List<SwapOperation>> CheckSwaps(string blockNumber, CancellationToken ct = default);
    Task<NetworkToken> GetTokenInfo(string address, CancellationToken ct = default);
    Task<List<SwapEvent>> GetSwapEvents(string txHash, CancellationToken ct = default);
    Task<List<PoolInfo>> GetLiquidityPools(LiquidityPoolsRequest request, CancellationToken ct = default);

    Task<ChainSubscription> SubscribeLogs<T>(
        string wsUrl, Action<WrappedEvent<T>> action, CancellationToken ct = default)
        where T : IEventDTO, new();
}