using dex.monitor.Business.DataStores.Persistant;
using Flour.Commons.Models;
using Mapster;
using MediatR;

namespace dex.monitor.Business.Features.Settings.Dex;

public class GetDexSettings : IRequest<ResultList<DexSettingsResponse>>;

public class DexSettingsResponse
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Network { get; set; }
    public string RouterAddress { get; set; }
    public string FactoryAddress { get; set; }
    public decimal Fee { get; set; }
}

public class GetDexSettingsHandler(IPersistantStore store)
    : IRequestHandler<GetDexSettings, ResultList<DexSettingsResponse>>
{
    public async Task<ResultList<DexSettingsResponse>> Handle(
        GetDexSettings request, CancellationToken cancellationToken)
    {
        var data = await store.DexSettings.GetMany(
            e => e.Id != Guid.Empty, e => e, cancellationToken);
        return ResultList<DexSettingsResponse>.Ok(data.Select(e => e.Adapt<DexSettingsResponse>()));
    }
}