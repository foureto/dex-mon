using dex.monitor.Business.Chains;
using dex.monitor.Business.Chains.Models;
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

        var pools = await chainProvider.GetLiquidityPools(
            new LiquidityPoolsRequest { FactoryAddress = "0xcA143Ce32Fe78f1f7019d7d551a6402fC5350c73" }, ct);

        do
        {
            var chainStatus = await newStore.ChainStatuses.Get(e => e.Id == chainRef.Id, ct);
            if (!chainStatus.IsActive) return;

            var dexs = await newStore.DexSettings
                .GetMany(e => e.IsActive && e.Network == chainStatus.Network, ct);

            chainStatus.Block.Height += 1;
            
            var swaps = await chainProvider.CheckSwaps(chainStatus.Block.Height.ToString(), ct);
            
            await newStore.ChainStatuses.StoreAndSave(chainStatus, ct);
            //
            // var tasks = visitors.Select(async e => await e.ProcessTransaction(
            //     new ProcessTransactionRequest { Block = block, Dexs = dexs, ChainStatus = chainStatus }, ct));

            // await Task.WhenAll(tasks);
        } while (!ct.IsCancellationRequested);
    }
}