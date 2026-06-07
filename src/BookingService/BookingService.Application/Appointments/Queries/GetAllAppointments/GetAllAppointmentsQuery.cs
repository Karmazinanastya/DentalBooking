using MediatR;
using Shared.BuildingBlocks.Common;
using BookingService.Application.Appointments.Queries.GetPatientAppointments;

namespace BookingService.Application.Appointments.Queries.GetAllAppointments;

public sealed record GetAllAppointmentsQuery(
    Guid? ClinicId,
    DateOnly? Date
) : IRequest<Result<IReadOnlyList<AppointmentDto>>>;
