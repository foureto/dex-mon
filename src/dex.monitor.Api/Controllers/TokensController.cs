using dex.monitor.Business.Features.Tokens;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dex.monitor.Api.Controllers;

[ApiController]
[Route("api/tokens")]
public class TokensController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTokens([FromQuery] GetTokens query, CancellationToken ct)
        => this.Respond(await sender.Send(query, ct));
}