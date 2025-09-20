using dex.monitor.Business.Domain;

namespace dex.monitor.Business.DataStores.MemoryStores.TokensStore;

internal class InMemoryTokenStore : ITokensStore
{
    private readonly Dictionary<string, List<NetworkToken>> _store = new();

    public Task<List<NetworkToken>> GetTokens(string network = null)
    {
        return network == null
            ? Task.FromResult(_store.Values.SelectMany(e => e).ToList())
            : Task.FromResult(_store.TryGetValue(network, out var list) ? list : []);
    }

    public Task AddToken(NetworkToken token)
    {
        _store.TryAdd(token.Network, []);
        _store[token.Network] ??= [];
        _store[token.Network].Add(token);
        return Task.CompletedTask;
    }

    public Task<NetworkToken> GetToken(string network, string address)
    {
        _store.TryAdd(network, []);
        return Task.FromResult(_store[network].FirstOrDefault(e =>
            e.Address.Equals(address, StringComparison.OrdinalIgnoreCase)));
    }
}