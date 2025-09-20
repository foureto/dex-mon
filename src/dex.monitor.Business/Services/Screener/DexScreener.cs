using dex.monitor.Business.Helpers;
using dex.monitor.Business.Services.Screener.Models.ApiModels;
using dex.monitor.Business.Services.Screener.Models.Requests;
using dex.monitor.Business.Services.Screener.Models.Responses;
using Microsoft.Extensions.Logging;

namespace dex.monitor.Business.Services.Screener;

internal class DexScreener(IHttpClientFactory factory, ILogger<DexScreener> logger)
    : BasicHttpClient(factory.CreateClient(nameof(DexScreener)), logger), IDexScreener
{
    public async Task<List<DexPairResponse>> GetPairs(GetDexPairRequest request, CancellationToken ct = default)
    {
        if (request.TokenAddresses.Count == 0) return [];
        var chunksCount = Math.Ceiling((double)request.TokenAddresses.Count / 30);
        var chunks = request.TokenAddresses.Chunk(30);

        var result = new List<DexPairResponse>();
        foreach (var chunk in chunks)
        {
            var tokens = string.Join(',', chunk);
            var response = await Get<List<DexScreenerPair>>($"/tokens/v1/{request.Chain}/{tokens}", null, ct);
            if (!response.Success || response.Data == null) continue;

            result.AddRange(response.Data.Select(e => new DexPairResponse
            {
                DexName = e.Dex,
                BaseSymbol = e.Base.Symbol,
                BaseAddress = e.Base.Address,
                QuotedSymbol = e.Quoted.Symbol,
                QuotedAddress = e.Quoted.Address,
                LiquidityBase = e.Liquidity.InBase,
                LiquidityQuoted = e.Liquidity.InQuoted,
                PairAddress = e.PairAddress,
            }));
        }

        return result;
    }

    public async Task<List<DexPairResponse>> GetPairsByToken(
        GetPairsByTokenRequest request, CancellationToken ct = default)
    {
        var response = await Get<List<DexScreenerPair>>($"/token-pairs/v1/{request.Chain}/{request.Address}", null, ct);
        if (!response.Success || response.Data == null) return [];

        return response.Data.Where(e => e.Base != null && e.Quoted != null && e.Liquidity != null)
            .Select(e =>
                new DexPairResponse
                {
                    DexName = e.Dex,
                    BaseSymbol = e.Base.Symbol,
                    BaseAddress = e.Base.Address,
                    QuotedSymbol = e.Quoted.Symbol,
                    QuotedAddress = e.Quoted.Address,
                    LiquidityBase = e.Liquidity.InBase,
                    LiquidityQuoted = e.Liquidity.InQuoted,
                    PairAddress = e.PairAddress,
                }).ToList();
    }
}