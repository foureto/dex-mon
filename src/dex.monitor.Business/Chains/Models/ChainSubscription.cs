namespace dex.monitor.Business.Chains.Models;

public class ChainSubscription
{
    public string Key { get; set; }
    public IDisposable Client { get; set; }
    public IDisposable Subscription { get; set; }
}