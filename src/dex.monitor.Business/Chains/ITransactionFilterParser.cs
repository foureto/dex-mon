using dex.monitor.Business.Domain;
using Nethereum.RPC.Eth.DTOs;

namespace dex.monitor.Business.Chains;

public interface ITransactionFilterParser
{
    string Network { get; }
    public bool Acceptable(Transaction transaction);
    Task<List<SwapOperation>> Process(IChainProvider provider, Transaction tx, CancellationToken ct);
}