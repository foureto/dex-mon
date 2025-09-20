namespace dex.monitor.Business.Services.Screener.Models.Requests;

public class GetDexPairRequest
{
    public string Chain { get; set; }
    public List<string> TokenAddresses { get; set; } = [];
}