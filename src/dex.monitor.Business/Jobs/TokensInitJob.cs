using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using dex.monitor.Business.Services.Cex;
using dex.monitor.Business.Services.Cex.Models;
using dex.monitor.Business.Services.Screener;
using dex.monitor.Business.Services.Screener.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dex.monitor.Business.Jobs;

internal class TokensInitJob(IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = provider.CreateAsyncScope();
        await GetTokensFromCex(stoppingToken);

        // var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        // await GetTokensDexInfo(stoppingToken);
        // await mediator.Publish(new InitPairs(), stoppingToken);
        await Task.Yield();
    }

    private async Task GetTokensFromCex(CancellationToken ct)
    {
        await using var scope = provider.CreateAsyncScope();
        var persistantStore = scope.ServiceProvider.GetRequiredService<IPersistantStore>();

        var cexs = scope.ServiceProvider.GetServices<ICexClient>().ToList();
        if (cexs.Count == 0)
        {
            await base.StopAsync(ct);
            return;
        }

        var toAdd = new Dictionary<string, CexToken>();
        var existingTokens = (await persistantStore.CexTokens.GetMany(e => true, ct))
            .ToDictionary(e => e.Code, e => e);

        foreach (var cex in cexs)
        {
            var tokens = await cex.GetTokens();
            var pairs = await cex.GetPairs(ct);
            foreach (var token in tokens)
            {
                if (existingTokens.TryGetValue(token.Code, out var existingToken))
                {
                    if (existingToken.Pairs.All(e => e.CexName != cex.Key))
                    {
                        existingToken.Pairs.AddRange(MapCexTokens(pairs, token));
                        persistantStore.CexTokens.Store(existingToken);
                    }

                    continue;
                }

                if (toAdd.TryGetValue(token.Code, out var tokenToAdd))
                {
                    if (tokenToAdd.Pairs.All(e => e.CexName != cex.Key))
                        tokenToAdd.Pairs.AddRange(MapCexTokens(pairs, token));

                    continue;
                }

                toAdd.Add(token.Code, new CexToken
                {
                    Code = token.Code,
                    Name = token.Name,
                    Pairs = MapCexTokens(pairs, token).ToList(),
                });
            }
        }

        await persistantStore.CexTokens.StoreAndSave(toAdd.Values, ct);
    }

    private static IEnumerable<CexPair> MapCexTokens(List<CexPairResponse> pairs, CexTokenInfoResponse token)
    {
        return pairs
            .Where(e => e.Base == token.Code || e.Quoted == token.Code)
            .Select(e => new CexPair
            {
                Base = e.Base,
                Quoted = e.Quoted,
                CexName = e.CexName,
                ApiSymbol = e.ApiSymbol,
                BaseIncrement = e.BaseIncrement,
                QuotedIncrement = e.QuotedIncrement,
                Fee = e.Fee,
                BaseMaxSize = e.BaseMaxSize,
                QuotedMaxSize = e.QuotedMaxSize,
                BaseMinSize = e.BaseMinSize,
                QuotedMinSize = e.QuotedMinSize,
                Networks = token.Networks.Select(n => new CexTokenNetwork
                {
                    Address = n.Address,
                    Network = n.Network,
                }).ToList()
            });
    }

    private async Task GetTokensDexInfo(CancellationToken ct)
    {
        var allowedNetworks = new[] { "eth", "bsc", "celo" };
        await using var scope = provider.CreateAsyncScope();
        var persistantStore = scope.ServiceProvider.GetRequiredService<IPersistantStore>();
        var dexScreener = scope.ServiceProvider.GetRequiredService<IDexScreener>();

        var tokens = (await persistantStore.Tokens.GetMany(e => !e.DexSynced, ct))
            .Where(e => e.Networks.Any(n => allowedNetworks.Contains(n.Network))).ToList();
        var tokensBySymbol = tokens.GroupBy(e => e.Code).ToDictionary(e => e.Key, e => e.First());

        foreach (var addressNet in tokensBySymbol.Values
                     .SelectMany(token => token.Networks
                         .Where(e => allowedNetworks.Contains(e.Network))
                         .Select(e => new { e.Address, e.Network })))
        {
            var response = await dexScreener.GetPairsByToken(
                new GetPairsByTokenRequest { Chain = addressNet.Network, Address = addressNet.Address }, ct);
            if (response.Count == 0) continue;

            var toUpdate = new List<TokenInfo>();
            foreach (var pair in response)
            {
                var pairToAdd = new DexPair
                {
                    DexName = pair.DexName,
                    PairAddress = pair.PairAddress,
                    BaseSymbol = pair.BaseSymbol,
                    BaseAddress = pair.BaseAddress,
                    QuotedSymbol = pair.QuotedSymbol,
                    QuotedAddress = pair.QuotedAddress,
                };

                NetworkToken tokenNet;
                if (tokensBySymbol.TryGetValue(pair.BaseSymbol, out var baseToken))
                {
                    tokenNet = baseToken.Networks.FirstOrDefault(e => e.Network == addressNet.Network);
                    if (tokenNet != null)
                    {
                        baseToken.IsValuable = true;
                        baseToken.DexSynced = true;
                        tokenNet.IsValuable = true;
                        tokenNet.Pairs ??= [];
                        if (tokenNet.Pairs.Any(e => e.PairAddress != pairToAdd.PairAddress))
                            tokenNet.Pairs.Add(pairToAdd);
                        toUpdate.Add(baseToken);
                    }
                }

                if (!tokensBySymbol.TryGetValue(pair.QuotedSymbol, out var quotedToken)) continue;

                tokenNet = quotedToken.Networks.FirstOrDefault(e => e.Network == addressNet.Network);
                if (tokenNet == null) continue;

                quotedToken.IsValuable = true;
                quotedToken.DexSynced = true;
                tokenNet.IsValuable = true;
                tokenNet.Pairs ??= [];
                if (tokenNet.Pairs.Any(e => e.PairAddress != pairToAdd.PairAddress))
                    tokenNet.Pairs.Add(pairToAdd);
                toUpdate.Add(quotedToken);
            }

            if (toUpdate.Count == 0) continue;

            await persistantStore.Tokens.StoreAndSave(toUpdate, ct);
        }
    }
}