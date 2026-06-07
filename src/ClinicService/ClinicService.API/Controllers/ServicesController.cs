using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using ClinicService.Application.Services.Commands.CreateService;
using ClinicService.Application.Services.Queries.GetServices;

namespace ClinicService.API.Controllers;

[ApiController]
[Route("services")]
public sealed class ServicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetServices(CancellationToken ct)
    {
        var result = await mediator.Send(new GetServicesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return result.ToActionResult();
        return Created(string.Empty, result.Value);
    }
}
