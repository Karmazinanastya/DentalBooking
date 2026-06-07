using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using ClinicService.Application.Doctors.Commands.CreateDoctor;
using ClinicService.Application.Doctors.Commands.SetSchedule;
using ClinicService.Application.Doctors.Queries.GetAvailableSlots;
using ClinicService.Application.Doctors.Queries.GetDoctorsByClinic;

namespace ClinicService.API.Controllers;

[ApiController]
[Route("doctors")]
public sealed class DoctorsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByClinic([FromQuery] Guid clinicId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetDoctorsByClinicQuery(clinicId), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return result.ToActionResult();
        return Created(string.Empty, result.Value);
    }

    [HttpPut("{doctorId:guid}/schedule")]
    public async Task<IActionResult> SetSchedule(
        Guid doctorId,
        [FromBody] SetDoctorScheduleCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command with { DoctorId = doctorId }, ct);
        return result.ToActionResult();
    }

    [HttpGet("{doctorId:guid}/slots")]
    public async Task<IActionResult> GetAvailableSlots(
        Guid doctorId,
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetAvailableSlotsQuery(doctorId, date), ct);
        return result.ToActionResult();
    }
}
