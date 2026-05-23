using MediatR;
using Shared.BuildingBlocks.Common;

namespace BookingService.Application.Appointments.Queries.GetPatientAppointments;

public sealed record GetPatientAppointmentsQuery(Guid PatientId) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;
