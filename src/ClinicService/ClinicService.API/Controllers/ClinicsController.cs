using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using ClinicService.Application.Clinics.Commands.CreateClinic;
using ClinicService.Application.Clinics.Queries.GetClinics;

namespace ClinicService.API.Controllers;

[ApiController]
[Route("clinics")]
public sealed class ClinicsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetClinics([FromQuery] string? city, CancellationToken ct)
    {
        var result = await mediator.Send(new GetClinicsQuery(city), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateClinic([FromBody] CreateClinicCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return result.ToActionResult();
        return Created(string.Empty, result.Value);
    }
}
