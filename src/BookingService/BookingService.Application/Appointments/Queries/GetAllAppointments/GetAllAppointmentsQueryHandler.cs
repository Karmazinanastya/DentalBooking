using MediatR;
using Shared.BuildingBlocks.Common;
using BookingService.Application.Appointments.Queries.GetPatientAppointments;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Queries.GetAllAppointments;

internal sealed class GetAllAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<GetAllAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(
        GetAllAppointmentsQuery request, CancellationToken ct)
    {
        var appointments = await appointmentRepository.GetAllAsync(request.ClinicId, request.Date, request.DoctorId, ct);

        var dtos = appointments.Select(a =>
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(a.ClinicTimeZoneId);
            var localDate = TimeZoneInfo.ConvertTimeFromUtc(a.AppointmentDateUtc, tz);
            return new AppointmentDto(
                a.Id,
                a.DoctorId,
                a.DoctorFullName,
                a.ClinicName,
                a.ClinicAddress,
                a.ServiceName,
                a.AppointmentDateUtc,
                localDate.ToString("dd.MM.yyyy HH:mm"),
                a.Status,
                a.CreatedAtUtc);
        }).ToList();

        return dtos;
    }
}
