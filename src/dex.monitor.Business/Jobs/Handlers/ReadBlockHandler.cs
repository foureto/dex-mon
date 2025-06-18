using dex.monitor.Business.Chains;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using dex.monitor.Business.Services.BlockVisitors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace dex.monitor.Business.Jobs.Handlers;

public class ReadBlock : INotification;

public class ReadBlockHandler(
    IServiceProvider serviceProvider,
    IPersistantStore store) : INotificationHandler<ReadBlock>
{
    public async Task Handle(ReadBlock notification, CancellationToken cancellationToken)
    {
        var chains = await store.ChainStatuses.GetMany(e => e.IsActive, cancellationToken);
        if (chains.Count == 0) return;

        var tasks = chains.Select(async e => await ProcessBlocks(e, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task ProcessBlocks(ChainStatus chainRef, CancellationToken ct)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var newStore = scope.ServiceProvider.GetRequiredService<IPersistantStore>();
        var visitors = scope.ServiceProvider.GetServices<IBlockVisitor>().ToList();
        var chainProvider = scope.ServiceProvider.GetRequiredKeyedService<IChainProvider>(chainRef.Network);

        do
        {
            var chainStatus = await newStore.ChainStatuses.Get(e => e.Id == chainRef.Id, ct);
            if (!chainStatus.IsActive) return;

            var dexs = await newStore.DexSettings
                .GetMany(e => e.IsActive && e.Network == chainStatus.Network, ct);

            chainStatus.Block.Height += 1;
            await newStore.ChainStatuses.StoreAndSave(chainStatus, ct);
            // var block = await chainProvider.GetBlock(chainStatus.Block.Height, ct);
            // if (block == null)
            //     return;
            //
            // var tasks = visitors.Select(async e => await e.ProcessTransaction(
            //     new ProcessTransactionRequest { Block = block, Dexs = dexs, ChainStatus = chainStatus }, ct));

            // await Task.WhenAll(tasks);
        } while (!ct.IsCancellationRequested);
    }
}