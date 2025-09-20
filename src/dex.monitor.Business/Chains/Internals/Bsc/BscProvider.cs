using dex.monitor.Business.Chains.Internals.Eth;
using dex.monitor.Business.DataStores.MemoryStores;
using dex.monitor.Business.DataStores.MemoryStores.TokensStore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dex.monitor.Business.Chains.Internals.Bsc;

internal class BscProvider(
    IChainFactory factory,
    ITokensStore tokensStore,
    IOptions<BscSettings> options,
    ILogger<BscProvider> logger)
    : EthProvider(factory, tokensStore, options, logger)
{
    public override string Network => ChainConstants.Bsc;
}