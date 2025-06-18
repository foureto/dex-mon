namespace dex.monitor.Business.Domain;

public class ChainStatus : BaseEntity
{
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Network { get; set; }
    public string ApiUrl { get; set; }
    public string WsUrl { get; set; }

    public NetworkBlock Block { get; set; } = new();
}

public class NetworkBlock
{
    public ulong Height { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}