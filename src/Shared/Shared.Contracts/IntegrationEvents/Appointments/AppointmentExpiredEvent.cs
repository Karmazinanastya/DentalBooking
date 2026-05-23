namespace Shared.Contracts.IntegrationEvents.Appointments;

public sealed record AppointmentExpiredEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid AppointmentId,
    Guid SlotId
) : IIntegrationEvent;
