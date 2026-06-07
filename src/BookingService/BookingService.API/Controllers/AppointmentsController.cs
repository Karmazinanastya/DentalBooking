using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Extensions;
using BookingService.Application.Appointments.Commands.CancelAppointment;
using BookingService.Application.Appointments.Commands.CompleteAppointment;
using BookingService.Application.Appointments.Commands.ConfirmAppointment;
using BookingService.Application.Appointments.Commands.CreateAppointment;
using BookingService.Application.Appointments.Queries.GetAllAppointments;
using BookingService.Application.Appointments.Queries.GetPatientAppointments;
using BookingService.Domain.Enums;

namespace BookingService.API.Controllers;

[ApiController]
[Route("appointments")]
public sealed class AppointmentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return result.ToActionResult();
        return Created(string.Empty, result.Value);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, [FromQuery] Guid patientId, CancellationToken ct)
    {
        var result = await mediator.Send(new ConfirmAppointmentCommand(id, patientId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> CancelByPatient(Guid id, [FromQuery] Guid patientId, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelAppointmentByPatientCommand(id, patientId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/cancel-by-clinic")]
    public async Task<IActionResult> CancelByClinic(
        Guid id, [FromBody] CancelByClinicRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CancelAppointmentByClinicCommand(id, request.ClinicId, request.Reason), ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromQuery] Guid clinicId, CancellationToken ct)
    {
        var result = await mediator.Send(new CompleteAppointmentCommand(id, clinicId), ct);
        return result.ToActionResult();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] Guid patientId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPatientAppointmentsQuery(patientId), ct);
        return result.ToActionResult();
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAppointments(
        [FromQuery] Guid? clinicId,
        [FromQuery] DateOnly? date,
        [FromQuery] Guid? doctorId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllAppointmentsQuery(clinicId, date, doctorId), ct);
        return result.ToActionResult();
    }
}

public sealed record CancelByClinicRequest(Guid ClinicId, string Reason);
