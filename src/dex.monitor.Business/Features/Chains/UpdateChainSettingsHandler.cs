using dex.monitor.Business.DataStores.Persistant;
using Flour.Commons.Models;
using Mapster;
using MediatR;

namespace dex.monitor.Business.Features.Chains;

public class UpdateChainSettings : IRequest<Result>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public string Network { get; set; }
    public string ApiUrl { get; set; }
    public string WsUrl { get; set; }
}

public class UpdateChainSettingsHandler(IPersistantStore store) : IRequestHandler<UpdateChainSettings, Result>
{
    public async Task<Result> Handle(UpdateChainSettings request, CancellationToken cancellationToken)
    {
        var existing = await store.ChainStatuses.Get(e => e.Id == request.Id, cancellationToken);
        if (existing == null) return Result.NotFound("Chain not found");
        
        request.Adapt(existing);
        existing.Updated = DateTimeOffset.UtcNow;
        
        await store.ChainStatuses.StoreAndSave(existing, cancellationToken);
        return Result.Ok("Dex updated");
    }
}