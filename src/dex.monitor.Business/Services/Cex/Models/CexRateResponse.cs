namespace dex.monitor.Business.Services.Cex.Models;

public class CexRateResponse
{
    public string Symbol { get; set; }
    public decimal Rate { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public DateTime Stamp { get; set; } = DateTime.UtcNow;
}