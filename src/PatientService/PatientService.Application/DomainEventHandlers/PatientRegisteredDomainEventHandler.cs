using MediatR;
using Shared.Contracts.Abstractions;
using PatientService.Domain.DomainEvents;
using Shared.Contracts.IntegrationEvents.Patients;

namespace PatientService.Application.DomainEventHandlers;

internal sealed class PatientRegisteredDomainEventHandler(IEventPublisher publisher)
    : INotificationHandler<PatientRegisteredDomainEvent>
{
    public Task Handle(PatientRegisteredDomainEvent e, CancellationToken ct) =>
        publisher.PublishAsync(new PatientRegisteredEvent(
            e.EventId, e.OccurredOn, e.PatientId, e.ChatId, e.FullName), ct);
}
