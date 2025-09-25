using dex.monitor.Business.Domain;
using Mapster;

namespace dex.monitor.Business.DataStores.MemoryStores.TokensStore;

internal class InMemoryTokenStore : ITokensStore
{
    private readonly Dictionary<string, Dictionary<string, NetworkToken>> _store = new();

    public Task<List<NetworkToken>> GetTokens(string network = null)
    {
        return Task.FromResult((
            network == null
                ? _store.Values.SelectMany(e => e.Values)
                : _store.TryGetValue(network, out var list)
                    ? list.Values
                    : []).Select(e => e.Adapt<NetworkToken>()).ToList());
    }

    public Task AddToken(NetworkToken token)
        => AddTokens([token]);

    public Task AddTokens(IEnumerable<NetworkToken> tokens)
    {
        foreach (var token in tokens.Where(e => !string.IsNullOrEmpty(e.Address)))
        {
            var newToken = token.Adapt<NetworkToken>();
            newToken.Address = token.Address.ToLower();
            _store.TryAdd(token.Network, new Dictionary<string, NetworkToken>());
            _store[token.Network] ??= new Dictionary<string, NetworkToken>();
            _store[token.Network].TryAdd(token.Address, newToken);
            _store[token.Network][token.Address] = newToken;
        }

        return Task.CompletedTask;
    }

    public Task<NetworkToken> GetToken(string network, string address)
    {
        _store.TryAdd(network, new Dictionary<string, NetworkToken>());
        return Task.FromResult(_store[network].GetValueOrDefault(address.ToLower())?.Adapt<NetworkToken>());
    }
}