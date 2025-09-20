using dex.monitor.Business.Domain;
using dex.monitor.Business.Services.Cex.Models;

namespace dex.monitor.Business.Services.Cex;

public interface ICexClient
{
    string Key { get; }
    Task<List<CexTokenInfoResponse>> GetTokens();
    Task<List<CexPairResponse>> GetPairs(CancellationToken cancellationToken);
    Task<List<CexRateResponse>> GetRates(CancellationToken cancellationToken);
}