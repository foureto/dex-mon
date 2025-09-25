using dex.monitor.Business.Domain;

namespace dex.monitor.Business.DataStores.MemoryStores.TokensStore;

public interface ITokensStore
{
    Task<List<NetworkToken>> GetTokens(string network = null);
    Task AddToken(NetworkToken token);
    Task AddTokens(IEnumerable<NetworkToken> tokens);
    Task<NetworkToken> GetToken(string network, string address);
}