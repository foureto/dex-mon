using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using Flour.Commons.Models;
using Mapster;
using MediatR;

namespace dex.monitor.Business.Features.Chains;

public class AddChainSettings : IRequest<Result>
{
    public string Name { get; set; }
    public string Network { get; set; }
    public string ApiUrl { get; set; }
    public string WsUrl { get; set; }
}

public class AddChainSettingsHandler(IPersistantStore store) : IRequestHandler<AddChainSettings, Result>
{
    public async Task<Result> Handle(AddChainSettings request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<ChainStatus>();
        await store.ChainStatuses.StoreAndSave(entity, cancellationToken);

        return Result.Ok("New chain added");
    }
}