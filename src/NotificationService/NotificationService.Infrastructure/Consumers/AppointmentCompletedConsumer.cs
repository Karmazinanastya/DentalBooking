using MassTransit;
using NotificationService.Application.Abstractions;
using Shared.Contracts.IntegrationEvents.Appointments;

namespace NotificationService.Infrastructure.Consumers;

internal sealed class AppointmentCompletedConsumer(ITelegramSender sender)
    : IConsumer<AppointmentCompletedEvent>
{
    public Task Consume(ConsumeContext<AppointmentCompletedEvent> context) =>
        sender.SendTextAsync(context.Message.PatientChatId,
            "Дякуємо за візит до нашої клініки! Сподіваємося, що все пройшло добре. " +
            "Будемо раді бачити вас знову.",
            context.CancellationToken);
}
