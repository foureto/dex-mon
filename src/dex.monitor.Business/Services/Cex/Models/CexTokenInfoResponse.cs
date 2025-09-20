using dex.monitor.Business.Domain;

namespace dex.monitor.Business.Services.Cex.Models;

public class CexTokenInfoResponse
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string CexName { get; set; }
    public List<CexNetworkTokenInfo> Networks { get; set; } = [];
}

public class CexNetworkTokenInfo
{
    public string Network { get; set; }
    public string Address { get; set; }
}