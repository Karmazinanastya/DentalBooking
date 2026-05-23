namespace Shared.Contracts.IntegrationEvents.Patients;

public sealed record PatientRegisteredEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid PatientId,
    long ChatId,
    string FullName
) : IIntegrationEvent;
