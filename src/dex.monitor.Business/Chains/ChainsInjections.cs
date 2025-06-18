using dex.monitor.Business.Chains.Filters;
using dex.monitor.Business.Chains.Internals.Bsc;
using dex.monitor.Business.Chains.Internals.Celo;
using dex.monitor.Business.Chains.Internals.Eth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dex.monitor.Business.Chains;

internal static class ChainsInjections
{
    public static IServiceCollection AddChains(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddScoped<IChainFactory, ChainFactory>()
            .AddEthProvider<EthProvider, EthSettings>(configuration, "chains:eth", ChainConstants.Eth)
            .AddEthProvider<BscProvider, BscSettings>(configuration, "chains:bsc", ChainConstants.Bsc)
            .AddEthProvider<CeloProvider, CeloSettings>(configuration, "chains:celo", ChainConstants.Celo)
            
            // filters
            .AddKeyedSingleton<ITransactionFilterParser, PancakeFilterParser>(ChainConstants.Bsc);

    private static IServiceCollection AddEthProvider<T, TS>(
        this IServiceCollection services, IConfiguration configuration, string sectionName, string name)
        where TS : class
        where T : EthProvider
        => services
            .Configure<TS>(e => configuration.GetSection(sectionName).Bind(e))
            .AddKeyedScoped<IChainProvider, T>(name);
}