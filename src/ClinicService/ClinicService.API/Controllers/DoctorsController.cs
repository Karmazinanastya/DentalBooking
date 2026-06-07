using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using ClinicService.Application.Doctors.Commands.CreateDoctor;
using ClinicService.Application.Doctors.Commands.SetSchedule;
using ClinicService.Application.Doctors.Queries.GetAvailableSlots;
using ClinicService.Application.Doctors.Queries.GetDoctorsByClinic;
using ClinicService.Application.Doctors.Queries.GetDoctorSchedule;
using ClinicService.Application.Slots.Commands.GenerateSlots;

namespace ClinicService.API.Controllers;

[ApiController]
[Route("doctors")]
public sealed class DoctorsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDoctors(
        [FromQuery] Guid? clinicId,
        [FromQuery] Guid? serviceId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetDoctorsByClinicQuery(clinicId, serviceId), ct);
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

    [HttpGet("{doctorId:guid}/schedule")]
    public async Task<IActionResult> GetSchedule(Guid doctorId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetDoctorScheduleQuery(doctorId), ct);
        return result.ToActionResult();
    }

    [HttpPost("{doctorId:guid}/slots/generate")]
    public async Task<IActionResult> GenerateSlots(
        Guid doctorId, [FromBody] GenerateSlotsRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new GenerateSlotsCommand(doctorId, request.FromDate, request.ToDate), ct);
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

public sealed record GenerateSlotsRequest(DateOnly FromDate, DateOnly ToDate);
