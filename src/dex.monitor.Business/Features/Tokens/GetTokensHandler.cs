using System.Linq.Expressions;
using dex.monitor.Business.DataStores.Persistant;
using dex.monitor.Business.Domain;
using Flour.Commons;
using Flour.Commons.Models;
using MediatR;

namespace dex.monitor.Business.Features.Tokens;

public class GetTokens : IRequest<ResultList<TokenInfo>>
{
    public bool? IsValuable { get; set; }
    public string[] Cexs { get; set; }
    public string[] Dexs { get; set; }
}

public class GetTokensHandler(IPersistantStore store) : IRequestHandler<GetTokens, ResultList<TokenInfo>>
{
    public async Task<ResultList<TokenInfo>> Handle(GetTokens request, CancellationToken cancellationToken)
    {
        Expression<Func<TokenInfo, bool>> expr = e => true;
        expr = request.IsValuable.HasValue ? expr.And(e => e.IsValuable == request.IsValuable) : expr;
        expr = request.Cexs is { Length: > 0 } ? expr.And(e => e.Cexes.Any(c => request.Cexs.Contains(c))) : expr;
        expr = request.Dexs is { Length: > 0 }
            ? expr.And(e => e.Networks.Any(n => n.Pairs.Any(p => request.Dexs.Contains(p.DexName))))
            : expr;

        var tokens = await store.Tokens.GetMany(expr, cancellationToken);
        return ResultList<TokenInfo>.Ok(tokens);
    }
}