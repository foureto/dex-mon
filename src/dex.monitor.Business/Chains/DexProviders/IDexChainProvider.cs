using dex.monitor.Business.Chains.Models;

namespace dex.monitor.Business.Chains.DexProviders;

public interface IDexChainProvider
{
    Task<DexPairRate> GetRate(DexPairRequest request, CancellationToken ct = default);
}