using MediatR;
using Shared.BuildingBlocks.Common;
using BookingService.Domain.Repositories;

namespace BookingService.Application.Appointments.Queries.GetPatientAppointments;

internal sealed class GetPatientAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<GetPatientAppointmentsQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(
        GetPatientAppointmentsQuery request, CancellationToken ct)
    {
        var appointments = await appointmentRepository.GetByPatientAsync(request.PatientId, ct);

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
