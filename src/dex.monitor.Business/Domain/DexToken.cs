namespace dex.monitor.Business.Domain;

public class DexToken
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    
    public List<DexPair> Pairs { get; set; } = [];
}

public class DexPair
{
    public string DexName { get; set; }
    public string Network { get; set; }
    public string PairAddress { get; set; }
    public string BaseAddress { get; set; }
    public string QuotedAddress { get; set; }
    public string BaseSymbol { get; set; }
    public string QuotedSymbol { get; set; }
    public string BaseName { get; set; }
    public string QuotedName { get; set; }
}