using dex.monitor.Business.DataStores.Persistant;
using Flour.Commons.Models;
using Mapster;
using MediatR;

namespace dex.monitor.Business.Features.Settings.Dex;

public class UpdateDexSettings : IRequest<Result>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Network { get; set; }
    public string RouterAddress { get; set; }
    public string FactoryAddress { get; set; }
    public decimal Fee { get; set; }
}

public class UpdateDexSettingsHandler(IPersistantStore store) : IRequestHandler<UpdateDexSettings, Result>
{
    public async Task<Result> Handle(UpdateDexSettings request, CancellationToken cancellationToken)
    {
        var existing = await store.DexSettings.Get(e => e.Id == request.Id, cancellationToken);
        if (existing == null) return Result.NotFound("Dex not found");

        request.Adapt(existing);
        existing.Updated = DateTimeOffset.UtcNow;

        await store.DexSettings.StoreAndSave(existing, cancellationToken);
        return Result.Ok("Dex updated");
    }
}