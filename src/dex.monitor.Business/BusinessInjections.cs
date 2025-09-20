using dex.monitor.Business.Chains;
using dex.monitor.Business.DataStores;
using dex.monitor.Business.Jobs;
using dex.monitor.Business.Services.BlockVisitors;
using dex.monitor.Business.Services.Cex;
using dex.monitor.Business.Services.Cex.Internals;
using dex.monitor.Business.Services.Screener;
using dex.monitor.Business.Services.SpreadDetector;
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
            .AddCexs(configuration)
            .AddDexScreener(configuration)
            .AddVisitors()
            .AddTransient<ISpreadDetector, SimpleSpreadDetector>();

    private static IServiceCollection AddVisitors(this IServiceCollection services)
        => services
            .AddTransient<IBlockVisitor, SwapOperationVisitor>();

    private static IServiceCollection AddCexs(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<GateIoCexSettings>(e => configuration.GetSection("cexs:gate-io").Bind(e))
            .Configure<KucoinCexSettings>(e => configuration.GetSection("cexs:kucoin").Bind(e))
            .Configure<HtxCexSettings>(e => configuration.GetSection("cexs:htx").Bind(e))
            .AddScoped<ICexClient, GateIoCexClient>()
            .AddScoped<ICexClient, KucoinCexClient>()
            .AddScoped<ICexClient, HtxCexClient>()
            .AddKeyedScoped<ICexClient, GateIoCexClient>(GateIoCexClient.CexName)
            .AddKeyedScoped<ICexClient, KucoinCexClient>(KucoinCexClient.CexName)
            .AddKeyedScoped<ICexClient, HtxCexClient>(HtxCexClient.CexName);

    private static IServiceCollection AddDexScreener(
        this IServiceCollection services, IConfiguration _)
        => services
            .AddScoped<IDexScreener, DexScreener>()
            .AddHttpClient(nameof(DexScreener),
                client => { client.BaseAddress = new Uri("https://api.dexscreener.com/"); })
            .AddStandardResilienceHandler().Services;
}