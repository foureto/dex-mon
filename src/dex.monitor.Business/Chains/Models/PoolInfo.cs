namespace dex.monitor.Business.Chains.Models;

public class PoolInfo
{
    public string Token0 { get; set; }
    public string Token1 { get; set; }
    public string PairAddress { get; set; }
    public decimal Reserve0 { get; set; }
    public decimal Reserve1 { get; set; }
    public decimal TotalSupply { get; set; }
    public decimal Rate => GetExchangeRate(true);
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    private decimal GetExchangeRate(bool token0ToToken1)
    {
        if (Reserve0 == 0 || Reserve1 == 0) return 0;
        return token0ToToken1 ? Reserve1 / Reserve0 : Reserve0 / Reserve1;
    }
}