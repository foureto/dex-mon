using dex.monitor.Business.Jobs.Handlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dex.monitor.Business.Jobs;

internal class ChainMonitorWorker(IServiceProvider provider, ILogger<ChainMonitorWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            try
            {
                await DoWork(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Block read failed");
            }

            await Task.Delay(1000, stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
    }

    private async Task DoWork(CancellationToken ct)
    {
        await using var scope = provider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(new ReadBlock(), ct);
    }
}