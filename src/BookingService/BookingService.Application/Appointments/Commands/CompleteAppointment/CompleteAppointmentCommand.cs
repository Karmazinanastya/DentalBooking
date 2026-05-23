using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Commands.CompleteAppointment;

public sealed record CompleteAppointmentCommand(Guid AppointmentId, Guid ClinicId) : IRequest<Result>;
