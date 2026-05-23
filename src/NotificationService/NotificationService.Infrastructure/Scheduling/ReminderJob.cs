using NotificationService.Application.Abstractions;

namespace NotificationService.Infrastructure.Scheduling;

public sealed class ReminderJob(ITelegramSender sender)
{
    public async Task ExecuteAsync(long chatId, string message) =>
        await sender.SendTextAsync(chatId, message);
}
