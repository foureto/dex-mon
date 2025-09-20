using CryptoExchange.Net.Authentication;
using dex.monitor.Business.Domain;
using dex.monitor.Business.Services.Cex.Models;
using HTX.Net.Clients;
using HTX.Net.Enums;
using Microsoft.Extensions.Options;

namespace dex.monitor.Business.Services.Cex.Internals;

public class HtxCexSettings
{
    public string Hmac { get; set; }
    public string ApiSecret { get; set; }
}

internal class HtxCexClient(IOptions<HtxCexSettings> options) : ICexClient
{
    public const string CexName = "htx";
    public string Key => CexName;

    private readonly HTXRestClient _client = new()
    {
        ClientOptions =
        {
            ApiCredentials = new ApiCredentials(options.Value.Hmac, options.Value.ApiSecret)
        }
    };

    public async Task<List<CexTokenInfoResponse>> GetTokens()
    {
        var result = new List<CexTokenInfoResponse>();
        var assets = await _client.SpotApi.ExchangeData.GetAssetsAsync();

        var networks = await _client.SpotApi.ExchangeData.GetNetworksAsync(
            NetworkRequestFilter.AllDescriptions);

        var byNetwork = networks.Data.Where(e => e.Visible && e.DepositEnabled && e.WithdrawalEnabled)
            .GroupBy(e => e.Code).ToDictionary(e => e.Key, e => e.ToList());


        foreach (var asset in assets.Data
                     .Where(e => e.Visible &&
                                 e.DepositEnabled &&
                                 e.WithdrawEnabled &&
                                 !e.CountryDisabled))
        {
            var token = new CexTokenInfoResponse
            {
                Name = asset.DisplayName,
                Code = asset.AssetCode.ToUpper(),
                CexName = CexName,
            };

            if (!byNetwork.TryGetValue(token.Code, out var assetNetworks)) continue;

            token.Networks = assetNetworks.Select(e => new CexNetworkTokenInfo
            {
                Address = e.ContractAddress,
                Network = e.ContractNetwork.ToLower()
            }).ToList();

            result.Add(token);
        }

        return result;
    }

    public async Task<List<CexPairResponse>> GetPairs(CancellationToken cancellationToken)
    {
        var symbols = await _client.SpotApi.ExchangeData.GetSymbolsAsync(cancellationToken);

        return symbols.Data.Where(e => e.TradeEnabled).Select(e => new CexPairResponse
        {
            CexName = CexName,
            Base = e.BaseAsset,
            Quoted = e.QuoteAsset,
            ApiSymbol = e.Name,
            BaseIncrement = e.PricePrecision.Scale,
            QuotedIncrement = e.QuantityPrecision.Scale,
            Fee = 0.2m,
        }).ToList();
    }

    public async Task<List<CexRateResponse>> GetRates(CancellationToken cancellationToken)
    {
        var tickers = await _client.SpotApi.ExchangeData.GetTickersAsync(ct: cancellationToken);
        return tickers.Data.Ticks.Select(e => new CexRateResponse
        {
            Symbol = e.Symbol,
            Ask = e.BestAskPrice,
            Bid = e.BestBidPrice,
            Rate = e.LastTradePrice,
        }).DistinctBy(e => e.Symbol).ToList();
    }
}