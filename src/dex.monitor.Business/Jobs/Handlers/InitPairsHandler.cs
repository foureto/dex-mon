using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Services.Cex;
using MediatR;

namespace dex.monitor.Business.Jobs.Handlers;

public class InitPairs : INotification;

public class InitPairsHandler(IPersistantStore store, IEnumerable<ICexClient> cexes) : INotificationHandler<InitPairs>
{
    public async Task Handle(InitPairs notification, CancellationToken cancellationToken)
    {
        var tokenCodes = await store.Tokens.GetMany(e => e.IsValuable, e => e.Code, cancellationToken);
        foreach (var cex in cexes)
        {
            var pairs = await cex.GetPairs(cancellationToken);
        }
        
    }
}