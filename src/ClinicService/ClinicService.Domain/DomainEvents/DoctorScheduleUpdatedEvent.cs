using Shared.BuildingBlocks.Domain;

namespace ClinicService.Domain.DomainEvents;

public sealed record DoctorScheduleUpdatedEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid DoctorId,
    Guid ClinicId
) : IDomainEvent;
