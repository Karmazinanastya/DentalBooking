namespace Shared.Contracts.IntegrationEvents.Appointments;

public sealed record AppointmentCreatedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    Guid DoctorId,
    string DoctorFullName,
    Guid ClinicId,
    string ClinicName,
    string ClinicAddress,
    string ServiceName,
    DateTime AppointmentDateUtc,
    string ClinicTimeZoneId
) : IIntegrationEvent;
