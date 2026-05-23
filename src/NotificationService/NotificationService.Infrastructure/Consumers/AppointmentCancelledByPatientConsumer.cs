using MassTransit;
using NotificationService.Application.Abstractions;
using Shared.Contracts.IntegrationEvents.Appointments;

namespace NotificationService.Infrastructure.Consumers;

internal sealed class AppointmentCancelledByPatientConsumer(ITelegramSender sender)
    : IConsumer<AppointmentCancelledByPatientEvent>
{
    public Task Consume(ConsumeContext<AppointmentCancelledByPatientEvent> context)
    {
        var msg = context.Message;
        return sender.SendTextAsync(msg.PatientChatId,
            $"Ваш запис на {msg.AppointmentDateUtc:dd.MM.yyyy HH:mm} UTC скасовано. " +
            "Запис відкрито для інших пацієнтів.",
            context.CancellationToken);
    }
}
