using dex.monitor.Business.Chains;
using dex.monitor.Business.Chains.Models;
using dex.monitor.Business.Jobs.Consumers.Logs;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace dex.monitor.Business.Services.BlockVisitors;

public class SwapOperationVisitor(
    IMessageBus messageBus, 
    IChainFactory chainFactory,
    ILogger<SwapOperationVisitor> logger) : IBlockVisitor
{
    public async Task ProcessTransaction(ProcessTransactionRequest request, CancellationToken ct = default)
    {
        await Task.Yield();
        if (request.Block == null || request.Dexs.Count == 0) return;

        // var routers = request.Dexs
        //     .Where(e => !string.IsNullOrWhiteSpace(e.RouterAddress) && e.IsActive)
        //     .ToDictionary(e => e.RouterAddress.ToLower());
        //
        // if (routers.Count == 0) 
        //     return;
        //
        // foreach (var tx in request.Block.Transactions.Where(e => e.To != null && routers.ContainsKey(e.To.ToLower())))
        // {
        //     var dex = routers[tx.To.ToLower()];
        //     await messageBus.PublishAsync(new CommonUiLog
        //     {
        //         Message = $"Has {dex.Name}/{dex.Network} swap operation: {tx.TxHash} at {request.Block.Height}",
        //         Section = UiLogSection.Chains,
        //     });
        //
        //     logger.LogInformation("Has swap operation: {HashId} at {Block} DEX: {Dex}", 
        //         tx.TxHash, request.Block.Height, dex.Name);
        //
        //     await ProcessReceipt(tx, ct);
        // }
    }

    // private async Task ProcessReceipt(ChainTransaction tx, CancellationToken ct)
    // {
    //     await Task.Yield();
    //     var swapEvents = await chainFactory.GetProvider(tx.Network).GetSwapEvents(tx.TxHash, ct);
    // }
}