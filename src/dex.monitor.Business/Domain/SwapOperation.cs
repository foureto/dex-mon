namespace dex.monitor.Business.Domain;

public class SwapOperation
{
    public string Network { get; set; }
    public string RouterAddress { get; set; }
    public TokenSwapPair Pair { get; set; }
}