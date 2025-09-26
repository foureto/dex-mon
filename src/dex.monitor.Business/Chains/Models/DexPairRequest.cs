namespace dex.monitor.Business.Chains.Models;

public class DexPairRequest
{
    public string PairAddress { get; set; }
    public string BaseAddress { get; set; }
    public string QuotedAddress { get; set; }
    public string DexProtocol { get; set; }
    public string RouterAddress { get; set; }
}