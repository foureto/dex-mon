using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dex.monitor.Business.Jobs;

internal static class JobsInjections
{
    public static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
        => services
            // .AddHostedService<ChainMonitorWorker>()
            // .AddHostedService<SwapMonitorJob>()
            // .AddHostedService<TokensInitJob>()
            .AddHostedService<TestPairRateJob>()
        ;
}