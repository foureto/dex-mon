using System.Threading.Channels;
using dex.monitor.Business.Chains;
using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.DataStores.Persistant;
using Flour.Commons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dex.monitor.Business.Jobs;

public class SwapMonitorJob(IServiceProvider provider, ILogger<SwapMonitorJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = provider.CreateAsyncScope();
        var persistantStore = scope.ServiceProvider.GetRequiredService<IPersistantStore>();
        var chainFactory = scope.ServiceProvider.GetRequiredService<IChainFactory>();

        var chains = await persistantStore.ChainStatuses.GetMany(e => e.IsActive, stoppingToken);
        var channel = Channel.CreateUnbounded<WrappedEvent<SwapEvent>>();
        var writer = channel.Writer;
        foreach (var chain in chains)
        {
            var chainProvider = chainFactory.GetProvider(chain.Network);
            var subscription = await chainProvider.SubscribeLogs<SwapEvent>(
                chain.WsUrl, e => writer.TryWrite(e), stoppingToken);
        }

        await foreach (var swapEvent in channel.Reader.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation("Got swap event: {Event}", swapEvent.ToJson());
        }

        await base.StopAsync(stoppingToken);
    }
}