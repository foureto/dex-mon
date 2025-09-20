using System.ComponentModel;
using CryptoExchange.Net.Authentication;
using dex.monitor.Business.Services.Cex.Models;
using Kucoin.Net.Clients;
using Microsoft.Extensions.Options;

namespace dex.monitor.Business.Services.Cex.Internals;

public class KucoinCexSettings
{
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}

[Description(CexName)]
internal class KucoinCexClient(IOptions<KucoinCexSettings> options) : ICexClient, IDisposable
{
    public const string CexName = "kucoin";

    private readonly KucoinRestClient _client = new()
    {
        ClientOptions =
        {
            ApiCredentials = new ApiCredentials(options.Value.ApiKey, options.Value.ApiSecret)
        }
    };

    public string Key => CexName;

    public async Task<List<CexTokenInfoResponse>> GetTokens()
    {
        var assets = await _client.SpotApi.ExchangeData.GetAssetsAsync();
        return assets.Data
            .Where(e => e.IsDebitEnabled)
            .Select(e => new CexTokenInfoResponse
            {
                Name = e.Name,
                Code = e.Asset.ToUpper(),
                CexName = CexName,
                Networks = e.Networks.Select(n => new CexNetworkTokenInfo
                {
                    Address = n.ContractAddress,
                    Network = n.NetworkId.ToLower()
                }).ToList()
            }).ToList();
    }

    public async Task<List<CexPairResponse>> GetPairs(CancellationToken cancellationToken)
    {
        var symbols = await _client.SpotApi.ExchangeData.GetSymbolsAsync(ct: cancellationToken);

        return symbols.Data.Select(e => new CexPairResponse
        {
            CexName = CexName,
            Base = e.BaseAsset,
            Quoted = e.QuoteAsset,
            ApiSymbol = e.Name,
            BaseIncrement = e.BaseIncrement.Scale,
            QuotedIncrement = e.QuoteIncrement.Scale,
            BaseMaxSize = e.BaseMaxQuantity,
            BaseMinSize = e.BaseMinQuantity,
            QuotedMaxSize = e.BaseMaxQuantity,
            QuotedMinSize = e.QuoteMinQuantity,
            Fee = Math.Max(e.TakerFeeCoefficient, e.MakerFeeCoefficient),
        }).ToList();
    }

    public async Task<List<CexRateResponse>> GetRates(CancellationToken cancellationToken)
    {
        var tickers = await _client.SpotApi.ExchangeData.GetTickersAsync(ct: cancellationToken);
        return tickers.Data.Data.Where(e => e.LastPrice.HasValue).Select(e => new CexRateResponse
        {
            Symbol = e.Symbol,
            Ask = e.BestAskPrice ?? e.LastPrice.GetValueOrDefault(),
            Bid = e.BestBidPrice ?? e.LastPrice.GetValueOrDefault(),
            Rate = e.LastPrice.GetValueOrDefault(),
        }).ToList();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}