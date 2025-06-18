using dex.monitor.Business.Chains;
using dex.monitor.Business.DataStores;
using dex.monitor.Business.Jobs;
using dex.monitor.Business.Services.BlockVisitors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dex.monitor.Business;

public static class BusinessInjections
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddMediatR(config => config.RegisterServicesFromAssembly(typeof(BusinessInjections).Assembly))
            .AddDataStores(configuration)
            .AddJobs(configuration)
            .AddChains(configuration)
            .AddVisitors();

    private static IServiceCollection AddVisitors(this IServiceCollection services)
        => services
            .AddTransient<IBlockVisitor, SwapOperationVisitor>();
}