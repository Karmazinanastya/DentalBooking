using BookingService.Domain.Enums;

namespace BookingService.Application.Appointments.Queries.GetPatientAppointments;

public sealed record AppointmentDto(
    Guid Id,
    string DoctorFullName,
    string ClinicName,
    string ClinicAddress,
    string ServiceName,
    DateTime AppointmentDateUtc,
    string LocalDateTime,
    AppointmentStatus Status,
    DateTime CreatedAtUtc
);
