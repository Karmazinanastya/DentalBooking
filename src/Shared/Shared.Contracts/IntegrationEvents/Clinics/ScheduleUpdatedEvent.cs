namespace Shared.Contracts.IntegrationEvents.Clinics;

public sealed record ScheduleUpdatedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid DoctorId,
    Guid ClinicId
) : IIntegrationEvent;
