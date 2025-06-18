using dex.monitor.Business.Features.Settings.Dex;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace dex.monitor.Api.Controllers;

[ApiController]
[Route("api/dexs")]
public class DexController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDexs(CancellationToken ct)
        => this.Respond(await sender.Send(new GetDexSettings(), ct));

    [HttpPost]
    public async Task<IActionResult> AddDex([FromBody] AddDexSettings request, CancellationToken ct)
        => this.Respond(await sender.Send(request, ct));

    [HttpPut]
    public async Task<IActionResult> UpdateDex([FromBody] UpdateDexSettings request, CancellationToken ct)
        => this.Respond(await sender.Send(request, ct));
}