using dex.monitor.Business.Chains.Internals.Eth;
using dex.monitor.Business.DataStores.MemoryStores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace dex.monitor.Business.Chains.Internals.Celo;

internal class CeloProvider(
    IChainFactory factory,
    ITokensStore tokensStore,
    IOptions<CeloSettings> options,
    ILogger<CeloProvider> logger)
    : EthProvider(factory, tokensStore, options, logger)
{
    public override string Network => ChainConstants.Celo;
}