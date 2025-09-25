using dex.monitor.Business.Chains;
using dex.monitor.Business.DataStores.MemoryStores.PairsStore;
using dex.monitor.Business.DataStores.MemoryStores.TokensStore;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using dex.monitor.Business.Jobs.Handlers;
using dex.monitor.Business.Services.Cex;
using dex.monitor.Business.Services.Cex.Models;
using dex.monitor.Business.Services.Screener;
using dex.monitor.Business.Services.Screener.Models.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dex.monitor.Business.Jobs;

internal class TokensInitJob(IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = provider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await GetTokensFromCex(stoppingToken);
        await mediator.Publish(new InitCexPairs(), stoppingToken);

        await GetDexTokens(stoppingToken);
        //await mediator.Publish(new InitDexPairs(), stoppingToken);

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

    private async Task GetDexTokens(CancellationToken ct)
    {
        await using var scope = provider.CreateAsyncScope();
        var tokensStore = scope.ServiceProvider.GetRequiredService<ITokensStore>();
        var pairsStore = scope.ServiceProvider.GetRequiredService<IPairsStore>();
        var persistantStore = scope.ServiceProvider.GetRequiredService<IPersistantStore>();
        var dexScreener = scope.ServiceProvider.GetRequiredService<IDexScreener>();

        var dexTokens = await persistantStore.DexTokens.GetMany(e => true, ct);
        var tokensBySymbol = dexTokens.GroupBy(e => e.Code).ToDictionary(e => e.Key, e => e.First());

        foreach (var network in ChainConstants.SupportedNetworks)
        {
            var networkTokens = await tokensStore.GetTokens(network);
            var toUpdate = new HashSet<DexToken>();
            var pairs = new List<SymbolRef>();
            foreach (var token in networkTokens)
            {
                var response = await dexScreener.GetPairsByToken(
                    new GetPairsByTokenRequest { Chain = network, Address = token.Address }, ct);
                if (response.Count == 0) continue;

                foreach (var pair in response)
                {
                    var dexPair = new DexPair
                    {
                        DexName = pair.DexName,
                        PairAddress = pair.PairAddress,
                        BaseSymbol = pair.BaseSymbol.ToUpperInvariant(),
                        BaseAddress = pair.BaseAddress,
                        BaseName = pair.BaseName,
                        QuotedSymbol = pair.QuotedSymbol.ToUpperInvariant(),
                        QuotedAddress = pair.QuotedAddress,
                        QuotedName = pair.QuotedName,
                        Network = network,
                    };

                    var toAdd = CheckPair(dexPair.BaseSymbol, dexPair, tokensBySymbol);
                    if (toAdd != null) toUpdate.Add(toAdd);

                    var nextToAdd = CheckPair(dexPair.QuotedSymbol, dexPair, tokensBySymbol);
                    if (nextToAdd != null) toUpdate.Add(nextToAdd);

                    pairs.Add(new SymbolRef(
                        dexPair.BaseSymbol,
                        dexPair.QuotedSymbol,
                        dexPair.PairAddress,
                        dexPair.DexName,
                        dexPair.Network,
                        pair.LiquidityBase / pair.LiquidityQuoted,
                        pair.LiquidityBase / pair.LiquidityQuoted,
                        DateTime.UtcNow));
                }
            }
            
            await pairsStore.SetPairs(pairs);

            if (toUpdate.Count == 0) continue;
            await persistantStore.DexTokens.StoreAndSave(toUpdate, ct);
        }
    }

    private static DexToken CheckPair(string code, DexPair pair, Dictionary<string, DexToken> tokensBySymbol)
    {
        var shouldReturn = false;
        if (!tokensBySymbol.TryGetValue(code, out var dexToken))
        {
            tokensBySymbol.TryAdd(code, new DexToken
            {
                Code = code,
                Name = pair.BaseSymbol == code ? pair.BaseName : pair.QuotedName,
                Address = pair.BaseAddress,
                Pairs = [pair],
            });
            shouldReturn = true;
        }

        dexToken = tokensBySymbol[code];
        if (dexToken.Pairs.Any(e => e.PairAddress == pair.PairAddress))
            return shouldReturn ? dexToken : null;

        dexToken.Pairs.Add(pair);

        return null;
    }
}