using MassTransit;
using NotificationService.Application.Abstractions;
using Shared.Contracts.IntegrationEvents.Appointments;

namespace NotificationService.Infrastructure.Consumers;

internal sealed class AppointmentCancelledByClinicConsumer(ITelegramSender sender)
    : IConsumer<AppointmentCancelledByClinicEvent>
{
    public Task Consume(ConsumeContext<AppointmentCancelledByClinicEvent> context)
    {
        var msg = context.Message;
        return sender.SendTextAsync(msg.PatientChatId,
            $"На жаль, клініка скасувала ваш запис на {msg.AppointmentDateUtc:dd.MM.yyyy HH:mm} UTC.\n" +
            $"Причина: {msg.Reason}\n\n" +
            "Ви можете записатися на інший час через меню.",
            context.CancellationToken);
    }
}
