using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Commands.ConfirmAppointment;

public sealed record ConfirmAppointmentCommand(Guid AppointmentId, Guid PatientId) : IRequest<Result>;
