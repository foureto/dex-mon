using dex.monitor.Business.Features.Chains;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dex.monitor.Api.Controllers;

[ApiController]
[Route("api/chains")]
public class ChainsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetChains(CancellationToken ct)
        => this.Respond(await sender.Send(new GetChainStatus(), ct));
    
    [HttpPost]
    public async Task<IActionResult> AddChain([FromBody] AddChainSettings request, CancellationToken ct)
        => this.Respond(await sender.Send(request, ct));

    [HttpPut]
    public async Task<IActionResult> UpdateChain([FromBody] UpdateChainSettings request, CancellationToken ct)
        => this.Respond(await sender.Send(request, ct));
    
    [HttpPut("block")]
    public async Task<IActionResult> UpdateChainBlock([FromBody] UpdateChainBlock request, CancellationToken ct)
        => this.Respond(await sender.Send(request, ct));
}