using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using Flour.Commons.Models;
using MediatR;

namespace dex.monitor.Business.Features.Chains;

public class UpdateChainBlock : IRequest<Result>
{
    public Guid ChainId { get; set; }
    public ulong Height { get; set; }
}

public class UpdateChainBlockHandler(IPersistantStore store) : IRequestHandler<UpdateChainBlock, Result>
{
    public async Task<Result> Handle(UpdateChainBlock request, CancellationToken cancellationToken)
    {
        var existing = await store.ChainStatuses.Get(e => e.Id == request.ChainId, cancellationToken);
        if (existing == null) return Result.NotFound("Chain not found");
        existing.Block ??= new NetworkBlock();
        existing.Block.Height = request.Height;
        existing.Block.UpdatedAt = DateTimeOffset.UtcNow;

        await store.ChainStatuses.StoreAndSave(existing, cancellationToken);
        return Result.Ok("Chain block updated");
    }
}