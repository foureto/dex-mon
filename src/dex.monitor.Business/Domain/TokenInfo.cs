namespace dex.monitor.Business.Domain;

public class TokenInfo
{
    public bool IsValuable { get; set; }
    public string Network { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Decimals { get; set; }
}