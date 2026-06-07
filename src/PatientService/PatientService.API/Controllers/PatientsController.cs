using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using PatientService.Application.Patients.Commands.RegisterPatient;
using PatientService.Application.Patients.Commands.UpdatePatient;
using PatientService.Application.Patients.Queries.GetPatient;

namespace PatientService.API.Controllers;

[ApiController]
[Route("patients")]
public sealed class PatientsController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterPatientCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return result.ToActionResult();
        return result.ToCreatedResult("GetPatientById", new { id = result.Value });
    }

    [HttpGet("{id:guid}", Name = "GetPatientById")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPatientByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpGet("by-telegram/{chatId:long}")]
    public async Task<IActionResult> GetByChatId(long chatId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPatientByChatIdQuery(chatId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdatePatientCommand(id, request.FirstName, request.LastName), ct);
        return result.ToActionResult();
    }
}

public sealed record UpdatePatientRequest(string FirstName, string LastName);
