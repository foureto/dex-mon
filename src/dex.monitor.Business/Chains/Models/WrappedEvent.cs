using Nethereum.ABI.FunctionEncoding.Attributes;

namespace dex.monitor.Business.Chains.Models;

public class WrappedEvent<T> where T : IEventDTO, new()
{
    public long BlockNumber { get; set; }
    public string TxId { get; set; }
    public int LogIndex { get; set; }
    public T AnEvent { get; set; }
}