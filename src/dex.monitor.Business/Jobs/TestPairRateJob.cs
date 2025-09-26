using dex.monitor.Business.Chains;
using dex.monitor.Business.Chains.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dex.monitor.Business.Jobs;

internal class TestPairRateJob(IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = provider.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<IChainFactory>();

        var chainProvider = factory.GetProvider("bsc");

        var rate = await chainProvider.GetPairRate(new DexPairRequest
        {
            BaseAddress = "0x55d398326f99059fF775485246999027B3197955",
            QuotedAddress = "0xbb73BB2505AC4643d5C0a99c2A1F34B3DfD09D11",
            PairAddress = "0xad74600521100F133Ee9006f2B1da7F0d5C98C80",
            RouterAddress = "0x4752ba5dbc23f44d87826276bf6fd6b1c372ad24",
            DexProtocol = ChainConstants.DexUniV2,
        }, stoppingToken);
    }
}