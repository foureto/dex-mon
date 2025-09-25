using System.Linq.Expressions;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using Flour.Commons.Models;
using MediatR;

namespace dex.monitor.Business.Features.Tokens;

public class GetTokens : IRequest<ResultList<DexToken>>
{
    public string[] Cexs { get; set; }
    public string[] Dexs { get; set; }
}

public class GetTokensHandler(IPersistantStore store) : IRequestHandler<GetTokens, ResultList<DexToken>>
{
    public async Task<ResultList<DexToken>> Handle(GetTokens request, CancellationToken cancellationToken)
    {
        Expression<Func<DexToken, bool>> expr = e => true;
        var tokens = await store.DexTokens.GetMany(expr, cancellationToken);
        return ResultList<DexToken>.Ok(tokens);
    }
}