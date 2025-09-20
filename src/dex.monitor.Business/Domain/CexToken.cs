namespace dex.monitor.Business.Domain;

public class CexToken
{
    public string Code { get; set; }
    public string Name { get; set; }
    public List<CexPair> Pairs { get; set; } = [];
}

public class CexPair
{
    public string CexName { get; set; }
    public string ApiSymbol { get; set; }
    public string Base { get; set; }
    public string Quoted { get; set; }
    public decimal BaseMinSize { get; set; }
    public decimal BaseMaxSize { get; set; }
    public decimal QuotedMinSize { get; set; }
    public decimal QuotedMaxSize { get; set; }
    public decimal Fee { get; set; }
    public int BaseIncrement { get; set; }
    public int QuotedIncrement { get; set; }
    public List<CexTokenNetwork> Networks { get; set; } = [];
}

public class CexTokenNetwork
{
    public string Network { get; set; }
    public string Address { get; set; }
}