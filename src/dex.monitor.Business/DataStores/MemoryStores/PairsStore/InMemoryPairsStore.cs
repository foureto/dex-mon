using dex.monitor.Business.Domain;

namespace dex.monitor.Business.DataStores.MemoryStores.PairsStore;

internal class InMemoryPairsStore : IPairsStore
{
    private readonly Dictionary<string, Dictionary<string, HashSet<SymbolRef>>> _symbols = new();

    public Task SetPair(SymbolRef symbol) => SetPairs([symbol]);

    public Task SetPairs(List<SymbolRef> symbols)
    {
        foreach (var symbol in symbols)
        {
            _symbols.TryAdd(symbol.Base, new Dictionary<string, HashSet<SymbolRef>>());
            _symbols[symbol.Base].TryAdd(symbol.Quoted, []);
            _symbols[symbol.Base][symbol.Quoted].RemoveWhere(e => e == symbol);
            _symbols[symbol.Base][symbol.Quoted].Add(symbol);

            var reversed = symbol with
            {
                Base = symbol.Quoted,
                Quoted = symbol.Base,
                Ask = 1 / (symbol.Bid == 0 ? 1 : symbol.Bid),
                Bid = 1 / (symbol.Ask == 0 ? 1 : symbol.Ask),
            };
            _symbols.TryAdd(reversed.Base, new Dictionary<string, HashSet<SymbolRef>>());
            _symbols[reversed.Base].TryAdd(reversed.Quoted, []);
            _symbols[reversed.Base][reversed.Quoted].RemoveWhere(e => e == reversed);
            _symbols[reversed.Base][reversed.Quoted].Add(reversed);
        }

        return Task.CompletedTask;
    }
}