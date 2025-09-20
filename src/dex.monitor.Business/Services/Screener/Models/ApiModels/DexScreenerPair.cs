using System.Text.Json.Serialization;

namespace dex.monitor.Business.Services.Screener.Models.ApiModels;

public class DexScreenerPair
{
    [JsonPropertyName("chainId")] public string Chain { get; set; }
    [JsonPropertyName("dexId")] public string Dex { get; set; }
    [JsonPropertyName("pairAddress")] public string PairAddress { get; set; }
    [JsonPropertyName("baseToken")] public DexScreenerPairToken Base { get; set; }
    [JsonPropertyName("quoteToken")] public DexScreenerPairToken Quoted { get; set; }
    [JsonPropertyName("liquidity")] public DexScreenerPairLiquidity Liquidity { get; set; }
    
}

public class DexScreenerPairToken
{
    [JsonPropertyName("address")] public string Address { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("symbol")] public string Symbol { get; set; }
}

public class DexScreenerPairLiquidity
{
    [JsonPropertyName("usd")] public decimal InUsd { get; set; }
    [JsonPropertyName("base")] public decimal InBase { get; set; }
    [JsonPropertyName("quote")] public decimal InQuoted { get; set; }
}