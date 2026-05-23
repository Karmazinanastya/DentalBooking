using MediatR;
using Shared.Contracts.Abstractions;
using BookingService.Domain.DomainEvents;
using Shared.Contracts.IntegrationEvents.Appointments;

namespace BookingService.Application.DomainEventHandlers;

internal sealed class AppointmentBookedDomainEventHandler(IEventPublisher publisher)
    : INotificationHandler<AppointmentBookedDomainEvent>
{
    public Task Handle(AppointmentBookedDomainEvent e, CancellationToken ct) =>
        publisher.PublishAsync(new AppointmentCreatedEvent(
            e.EventId, e.OccurredOn, e.AppointmentId, e.PatientId, e.PatientChatId,
            e.DoctorId, e.DoctorFullName, e.ClinicId, e.ClinicName, e.ClinicAddress,
            e.ServiceName, e.AppointmentDateUtc, e.ClinicTimeZoneId), ct);
}

internal sealed class AppointmentCancelledByPatientDomainEventHandler(IEventPublisher publisher)
    : INotificationHandler<AppointmentCancelledByPatientDomainEvent>
{
    public Task Handle(AppointmentCancelledByPatientDomainEvent e, CancellationToken ct) =>
        publisher.PublishAsync(new AppointmentCancelledByPatientEvent(
            e.EventId, e.OccurredOn, e.AppointmentId, e.PatientId, e.PatientChatId, e.AppointmentDateUtc), ct);
}

internal sealed class AppointmentCancelledByClinicDomainEventHandler(IEventPublisher publisher)
    : INotificationHandler<AppointmentCancelledByClinicDomainEvent>
{
    public Task Handle(AppointmentCancelledByClinicDomainEvent e, CancellationToken ct) =>
        publisher.PublishAsync(new AppointmentCancelledByClinicEvent(
            e.EventId, e.OccurredOn, e.AppointmentId, e.PatientId, e.PatientChatId,
            e.AppointmentDateUtc, e.Reason), ct);
}

internal sealed class AppointmentCompletedDomainEventHandler(IEventPublisher publisher)
    : INotificationHandler<AppointmentCompletedDomainEvent>
{
    public Task Handle(AppointmentCompletedDomainEvent e, CancellationToken ct) =>
        publisher.PublishAsync(new AppointmentCompletedEvent(
            e.EventId, e.OccurredOn, e.AppointmentId, e.PatientId, e.PatientChatId,
            e.DoctorId, e.ClinicId), ct);
}

internal sealed class AppointmentExpiredDomainEventHandler(IEventPublisher publisher)
    : INotificationHandler<AppointmentExpiredDomainEvent>
{
    public Task Handle(AppointmentExpiredDomainEvent e, CancellationToken ct) =>
        publisher.PublishAsync(new AppointmentExpiredEvent(
            e.EventId, e.OccurredOn, e.AppointmentId, e.SlotId), ct);
}
