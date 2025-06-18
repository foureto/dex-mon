using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using Flour.Commons.Models;
using Mapster;
using MediatR;

namespace dex.monitor.Business.Features.Settings.Dex;

public class AddDexSettings : IRequest<Result>
{
    public string Name { get; set; }
    public string Network { get; set; }
    public string RouterAddress { get; set; }
    public string FactoryAddress { get; set; }
    public decimal Fee { get; set; }
}

public class AddDexSettingsHandler(IPersistantStore store) : IRequestHandler<AddDexSettings, Result>
{
    public async Task<Result> Handle(AddDexSettings request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<DexSettings>();
        await store.DexSettings.StoreAndSave(entity, cancellationToken);
        
        return Result.Ok("New dex added");
    }
}