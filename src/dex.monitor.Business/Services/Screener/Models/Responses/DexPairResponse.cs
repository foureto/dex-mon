namespace dex.monitor.Business.Services.Screener.Models.Responses;

public class DexPairResponse
{
    public string DexName { get; set; }
    public string PairAddress { get; set; }
    public string BaseSymbol { get; set; }
    public string BaseAddress { get; set; }
    public string QuotedSymbol { get; set; }
    public string QuotedAddress { get; set; }
    public decimal LiquidityBase { get; set; }
    public decimal LiquidityQuoted { get; set; }
}