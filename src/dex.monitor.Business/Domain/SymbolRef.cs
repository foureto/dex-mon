namespace dex.monitor.Business.Domain;

public record SymbolRef(
    string Base,
    string Quoted,
    string ApiCode,
    string Exchange,
    string Network,
    decimal Bid,
    decimal Ask,
    DateTime Stamp)
{
    public override int GetHashCode() => HashCode.Combine(ApiCode, Exchange, Network);
}