using dex.monitor.Business.Domain;

namespace dex.monitor.Business.DataStores.Persistant;

public interface IPersistantStore
{
    IRepo<ChainStatus> ChainStatuses { get; }
    IRepo<DexSettings> DexSettings { get; }
    IRepo<DexToken> DexTokens { get; }
    IRepo<CexToken> CexTokens { get; }
}