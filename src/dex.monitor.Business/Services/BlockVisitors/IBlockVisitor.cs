using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.Domain;

namespace dex.monitor.Business.Services.BlockVisitors;

public interface IBlockVisitor
{
    Task ProcessTransaction(ProcessTransactionRequest request, CancellationToken ct = default);
}

public class ProcessTransactionRequest
{
    public TokenSwapPair Block { get; set; }
    public ChainStatus ChainStatus { get; set; }
    public IReadOnlyList<DexSettings> Dexs { get; set; } = [];
}