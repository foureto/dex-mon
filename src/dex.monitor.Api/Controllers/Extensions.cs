
using Microsoft.AspNetCore.Mvc;
using IResult = Flour.Commons.Models.IResult;

namespace dex.monitor.Api.Controllers;

public static class Extensions
{
    public static IActionResult Respond(this ControllerBase controllerBase, IResult result)
    {
        return controllerBase.StatusCode(result.StatusCode, result);
    }
}