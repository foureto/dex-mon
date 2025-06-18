using dex.monitor.Business.Domain;

namespace dex.monitor.Business.DataStores.MemoryStores;

public interface ITokensStore
{
    Task<List<TokenInfo>> GetTokens(string network = null);
    Task AddToken(TokenInfo token);
    Task<TokenInfo> GetToken(string network, string address);
}