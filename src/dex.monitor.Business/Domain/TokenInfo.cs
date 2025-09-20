namespace dex.monitor.Business.Domain;

public class TokenInfo
{
    public bool IsValuable { get; set; }
    public bool DexSynced { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public List<string> Cexes { get; set; } = [];
    public List<NetworkToken> Networks { get; set; } = [];
}

public class NetworkToken
{
    public bool IsValuable { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Network { get; set; }
    public string Address { get; set; }
    public int Decimals { get; set; }
    public List<DexPair> Pairs { get; set; } = [];
}

public class DexPair
{
    public string PairAddress { get; set; }
    public string BaseAddress { get; set; }
    public string QuotedAddress { get; set; }
    public string DexName { get; set; }
    public string BaseSymbol { get; set; }
    public string QuotedSymbol { get; set; }
}