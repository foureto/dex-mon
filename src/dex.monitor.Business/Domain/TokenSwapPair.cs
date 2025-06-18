namespace dex.monitor.Business.Domain;

public class TokenSwapPair
{
    public string PairAddress { get; set; }
    public string Symbol { get; set; }
    public TokenSwapValue In { get; set; }
    public TokenSwapValue Out { get; set; }
    public decimal Bid => In == null || Out == null ? 0m : In.Value / Out.Value;
    public decimal Ask => In == null || Out == null ? 0m : Out.Value / In.Value;
}