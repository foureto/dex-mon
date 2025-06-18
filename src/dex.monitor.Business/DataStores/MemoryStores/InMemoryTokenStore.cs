using dex.monitor.Business.Domain;
using JasperFx.Core;

namespace dex.monitor.Business.DataStores.MemoryStores;

internal class InMemoryTokenStore : ITokensStore
{
    private readonly Dictionary<string, List<TokenInfo>> _store = new();

    public Task<List<TokenInfo>> GetTokens(string network = null)
    {
        return network == null
            ? Task.FromResult(_store.Values.SelectMany(e => e).ToList())
            : Task.FromResult(_store.TryGetValue(network, out var list) ? list : []);
    }

    public Task AddToken(TokenInfo token)
    {
        _store[token.Network] ??= [];
        _store[token.Network].Add(token);

        return Task.CompletedTask;
    }

    public Task<TokenInfo> GetToken(string network, string address)
    {
        _store[network] ??= [];
        return Task.FromResult(_store[network].FirstOrDefault(e => e.Address.EqualsIgnoreCase(address)));
    }
}