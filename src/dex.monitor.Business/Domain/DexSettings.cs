namespace dex.monitor.Business.Domain;

public class DexSettings : BaseEntity
{
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Network { get; set; }
    public string RouterAddress { get; set; }
    public string FactoryAddress { get; set; }
    public decimal Fee { get; set; }
    public List<TokenSwapPair> Pairs { get; set; } = [];
}