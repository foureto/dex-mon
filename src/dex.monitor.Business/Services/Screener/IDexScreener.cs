using dex.monitor.Business.Services.Screener.Models.Requests;
using dex.monitor.Business.Services.Screener.Models.Responses;

namespace dex.monitor.Business.Services.Screener;

public interface IDexScreener
{
    Task<List<DexPairResponse>> GetPairs(GetDexPairRequest request, CancellationToken ct = default);
    Task<List<DexPairResponse>> GetPairsByToken(GetPairsByTokenRequest request, CancellationToken ct = default);
}