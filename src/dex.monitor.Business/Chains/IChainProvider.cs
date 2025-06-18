using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.Domain;

namespace dex.monitor.Business.Chains;

public interface IChainProvider
{
    string Network { get; }
    Task<List<SwapOperation>> CheckSwaps(string blockNumber, CancellationToken ct = default);
    Task<TokenInfo> GetTokenInfo(string address, CancellationToken ct = default);
    Task<List<SwapEvent>> GetSwapEvents(string txHash, CancellationToken ct = default);
}