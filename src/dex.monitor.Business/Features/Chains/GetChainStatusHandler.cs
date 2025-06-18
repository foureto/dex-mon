using dex.monitor.Business.DataStores.Persistant;
using Flour.Commons.Models;
using Mapster;
using MediatR;

namespace dex.monitor.Business.Features.Chains;

public class GetChainStatus : IRequest<ResultList<ChainStatusResponse>>;

public class ChainStatusResponse
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Network { get; set; }
    public string ApiUrl { get; set; }
    public string WsUrl { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public BlockResponse Block { get; set; }
}

public class BlockResponse
{
    public ulong Height { get; set; }
    public DateTimeOffset Updated { get; set; }
}

public class GetChainStatusHandler(IPersistantStore store) : IRequestHandler<GetChainStatus, ResultList<ChainStatusResponse>>
{
    public async Task<ResultList<ChainStatusResponse>> Handle(
        GetChainStatus request, CancellationToken cancellationToken)
    {
        var data = await store.ChainStatuses.GetMany(
            e => true, e => e, cancellationToken);
        return ResultList<ChainStatusResponse>.Ok(data.Select(e => e.Adapt<ChainStatusResponse>()));
    }
}