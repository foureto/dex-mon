using dex.monitor.Business.Chains;
using dex.monitor.Business.DataStores.MemoryStores.PairsStore;
using dex.monitor.Business.DataStores.MemoryStores.TokensStore;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using MediatR;

namespace dex.monitor.Business.Jobs.Handlers;

public class InitCexPairs : INotification;

public class InitCexPairsHandler(IPersistantStore store, IPairsStore pairsStore, ITokensStore tokensStore)
    : INotificationHandler<InitCexPairs>
{
    public async Task Handle(InitCexPairs notification, CancellationToken cancellationToken)
    {
        var cexTokens = await store.CexTokens.GetMany(e => true, cancellationToken);
        await pairsStore.SetPairs(cexTokens.SelectMany(e => e.Pairs).Select(e =>
            new SymbolRef(e.Base, e.Quoted, e.ApiSymbol, e.CexName, null, 0m, 0m, DateTime.UtcNow)).ToList());

        var potentialDexPairs = cexTokens.SelectMany(e =>
            e.Pairs.SelectMany(p =>
                p.Networks.Where(n => ChainConstants.SupportedNetworks.Contains(n.Network ?? string.Empty))
                    .Select(n => new NetworkToken
                    {
                        Network = n.Network,
                        Address = n.Address,
                        Code = e.Code,
                        Name = e.Name,
                    })
            ));

        await tokensStore.AddTokens(potentialDexPairs);
    }
}