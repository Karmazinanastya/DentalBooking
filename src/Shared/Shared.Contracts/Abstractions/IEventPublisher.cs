using Shared.Contracts.IntegrationEvents;

namespace Shared.Contracts.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default)
        where T : class, IIntegrationEvent;
}
