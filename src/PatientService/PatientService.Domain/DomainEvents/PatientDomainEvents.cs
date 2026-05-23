using Shared.BuildingBlocks.Domain;

namespace PatientService.Domain.DomainEvents;

public sealed record PatientRegisteredDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid PatientId,
    long ChatId,
    string FullName
) : IDomainEvent;
