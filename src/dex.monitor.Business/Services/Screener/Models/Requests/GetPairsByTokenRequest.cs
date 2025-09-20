namespace dex.monitor.Business.Services.Screener.Models.Requests;

public class GetPairsByTokenRequest
{
    public string Chain { get; set; }
    public string Address { get; set; }
}