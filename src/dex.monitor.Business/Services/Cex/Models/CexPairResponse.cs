namespace dex.monitor.Business.Services.Cex.Models;

public class CexPairResponse
{
    public string Base { get; set; }
    public string Quoted { get; set; }
    public string ApiSymbol { get; set; }
    public string CexName { get; set; }
    public decimal BaseMinSize { get; set; }
    public decimal BaseMaxSize { get; set; }
    public decimal QuotedMinSize { get; set; }
    public decimal QuotedMaxSize { get; set; }
    public decimal Fee { get; set; }
    public int BaseIncrement { get; set; }
    public int QuotedIncrement { get; set; }
}