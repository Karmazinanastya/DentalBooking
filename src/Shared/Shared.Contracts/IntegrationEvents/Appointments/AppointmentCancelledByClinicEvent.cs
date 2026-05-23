namespace Shared.Contracts.IntegrationEvents.Appointments;

public sealed record AppointmentCancelledByClinicEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid PatientId,
    long PatientChatId,
    DateTime AppointmentDateUtc,
    string Reason
) : IIntegrationEvent;
