using dex.monitor.Business.Domain;
using Marten;

namespace dex.monitor.Business.DataStores.Persistant.Internals;

internal class MartenPersistantStore(IDocumentStore store) : IPersistantStore
{
    public IRepo<ChainStatus> ChainStatuses => new BaseMartenRepo<ChainStatus>(store.LightweightSession());
    public IRepo<DexSettings> DexSettings => new BaseMartenRepo<DexSettings>(store.LightweightSession());
    public IRepo<DexToken> DexTokens => new BaseMartenRepo<DexToken>(store.LightweightSession());
    public IRepo<CexToken> CexTokens => new BaseMartenRepo<CexToken>(store.LightweightSession());
}