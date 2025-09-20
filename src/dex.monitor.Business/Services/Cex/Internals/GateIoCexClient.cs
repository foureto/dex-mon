using System.ComponentModel;
using CryptoExchange.Net.Authentication;
using dex.monitor.Business.Domain;
using dex.monitor.Business.Services.Cex.Models;
using GateIo.Net.Clients;
using Microsoft.Extensions.Options;

namespace dex.monitor.Business.Services.Cex.Internals;

public class GateIoCexSettings
{
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}

[Description(CexName)]
internal class GateIoCexClient(IOptions<GateIoCexSettings> options) : ICexClient, IDisposable
{
    public const string CexName = "gate-io";

    private readonly GateIoRestClient _client = new()
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
            .Where(e => !e.Delisted &&
                        !e.DepositDisabled &&
                        !e.TradeDisabled &&
                        !e.WithdrawDisabled &&
                        !e.WithdrawDelayed)
            .Select(e => new CexTokenInfoResponse
            {
                Name = e.Name,
                Code = e.Asset.ToUpper(),
                CexName = CexName,
                Networks = e.Networks.Select(n => new CexNetworkTokenInfo
                {
                    Address = n.Address,
                    Network = n.Name.ToLower()
                }).ToList()
            }).ToList();
    }

    public async Task<List<CexPairResponse>> GetPairs(CancellationToken cancellationToken)
    {
        var symbols = await _client.SpotApi.ExchangeData.GetSymbolsAsync(cancellationToken);

        return symbols.Data.Select(e => new CexPairResponse
        {
            CexName = CexName,
            Base = e.BaseAsset,
            Quoted = e.QuoteAsset,
            ApiSymbol = e.Name,
            BaseIncrement = e.PricePrecision,
            QuotedIncrement = e.PricePrecision,
            BaseMaxSize = e.MaxBaseQuantity.GetValueOrDefault(),
            BaseMinSize = e.MinBaseQuantity,
            QuotedMaxSize = e.MaxQuoteQuantity,
            QuotedMinSize = e.MinQuoteQuantity,
            Fee = e.TradeFee,
        }).ToList();
    }

    public async Task<List<CexRateResponse>> GetRates(CancellationToken cancellationToken)
    {
        var tickers = await _client.SpotApi.ExchangeData.GetTickersAsync(ct: cancellationToken);
        return tickers.Data.Select(e => new CexRateResponse
        {
            Symbol = e.Symbol,
            Ask = e.BestAskPrice ?? e.LastPrice,
            Bid = e.BestBidPrice ?? e.LastPrice,
            Rate = e.LastPrice,
        }).ToList();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}