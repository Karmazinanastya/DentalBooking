namespace Shared.Contracts.IntegrationEvents.Appointments;

public sealed record AppointmentCompletedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    Guid DoctorId,
    Guid ClinicId
) : IIntegrationEvent;
