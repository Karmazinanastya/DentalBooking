namespace NotificationService.Application.Abstractions;

public interface ITelegramSender
{
    Task SendTextAsync(long chatId, string text, CancellationToken ct = default);
}
