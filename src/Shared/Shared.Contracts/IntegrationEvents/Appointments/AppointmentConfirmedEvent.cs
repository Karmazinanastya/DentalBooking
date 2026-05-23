namespace Shared.Contracts.IntegrationEvents.Appointments;

public sealed record AppointmentConfirmedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId
) : IIntegrationEvent;
