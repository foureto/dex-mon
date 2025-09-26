namespace dex.monitor.Business.Chains.Models;

public class DexPairRate
{
    public string ExchangeName { get; set; } = string.Empty;
    public string TradingPair { get; set; } = string.Empty;
    public decimal BidPrice { get; set; }
    public decimal AskPrice { get; set; }
    public decimal Liquidity { get; set; } // For DEX pools
    public string Network { get; set; } // For DEX
    public DateTime Stamp { get; set; }
    
}