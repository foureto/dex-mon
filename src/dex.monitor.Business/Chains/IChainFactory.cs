using Microsoft.Extensions.DependencyInjection;

namespace dex.monitor.Business.Chains;

public interface IChainFactory
{
    IChainProvider GetProvider(string network);
    
    List<ITransactionFilterParser> GetFilterParsers(string network);
}

internal class ChainFactory(IServiceProvider provider) : IChainFactory
{
    public IChainProvider GetProvider(string network)
        => provider.GetRequiredKeyedService<IChainProvider>(network);

    public List<ITransactionFilterParser> GetFilterParsers(string network)
        => provider.GetKeyedServices<ITransactionFilterParser>(network).ToList();
}