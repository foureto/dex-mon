using dex.monitor.Business.Domain;

namespace dex.monitor.Business.DataStores.MemoryStores.PairsStore;

public interface IPairsStore
{
    Task SetPair(SymbolRef symbol);
    Task SetPairs(List<SymbolRef> symbols);
}