using dex.monitor.Business.Chains.Internals.Bsc;
using dex.monitor.Business.Chains.Internals.Eth;
using dex.monitor.Business.DataStores.MemoryStores.TokensStore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dex.monitor.Business.Chains.Internals.Polygon;

internal class PolygonProvider(
    IChainFactory factory,
    ITokensStore tokensStore,
    IOptions<PolygonSettings> options,
    ILogger<PolygonProvider> logger)
    : EthProvider(factory, tokensStore, options, logger)
{
    public override string Network => ChainConstants.Bsc;
}