using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Commands.CancelAppointment;

public sealed record CancelAppointmentByClinicCommand(
    Guid AppointmentId,
    Guid ClinicId,
    string Reason
) : IRequest<Result>;
