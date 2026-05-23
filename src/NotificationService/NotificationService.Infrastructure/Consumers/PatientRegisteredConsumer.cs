using MassTransit;
using NotificationService.Application.Abstractions;
using Shared.Contracts.IntegrationEvents.Patients;

namespace NotificationService.Infrastructure.Consumers;

internal sealed class PatientRegisteredConsumer(ITelegramSender sender)
    : IConsumer<PatientRegisteredEvent>
{
    public Task Consume(ConsumeContext<PatientRegisteredEvent> context)
    {
        var msg = context.Message;
        return sender.SendTextAsync(msg.ChatId,
            $"Ласкаво просимо, {msg.FullName}!\n\n" +
            "Ви успішно зареєструвалися у системі DentalBook. " +
            "Тепер ви можете записатися на прийом через меню.",
            context.CancellationToken);
    }
}
