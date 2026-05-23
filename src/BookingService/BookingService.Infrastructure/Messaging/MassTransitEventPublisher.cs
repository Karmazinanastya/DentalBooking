using MassTransit;
using Shared.Contracts.Abstractions;
using Shared.Contracts.IntegrationEvents;

namespace BookingService.Infrastructure.Messaging;

internal sealed class MassTransitEventPublisher(IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default)
        where T : class, IIntegrationEvent =>
        publishEndpoint.Publish(integrationEvent, ct);
}
