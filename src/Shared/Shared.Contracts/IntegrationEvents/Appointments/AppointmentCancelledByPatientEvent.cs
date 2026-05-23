namespace Shared.Contracts.IntegrationEvents.Appointments;

public sealed record AppointmentCancelledByPatientEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    DateTime AppointmentDateUtc
) : IIntegrationEvent;
