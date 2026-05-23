using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Commands.CancelAppointment;

public sealed record CancelAppointmentByPatientCommand(Guid AppointmentId, Guid PatientId) : IRequest<Result>;
