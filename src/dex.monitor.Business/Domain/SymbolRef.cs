namespace dex.monitor.Business.Domain;

public record SymbolRef(
    string Base,
    string Quoted,
    string Exchange,
    string Network,
    decimal Bid,
    decimal Ask,
    DateTime Stamp)
{
    public override int GetHashCode() => HashCode.Combine(Base, Quoted, Exchange, Network);
}