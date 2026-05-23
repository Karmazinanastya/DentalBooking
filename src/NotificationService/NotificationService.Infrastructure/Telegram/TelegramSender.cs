using Telegram.Bot;
using NotificationService.Application.Abstractions;

namespace NotificationService.Infrastructure.Telegram;

internal sealed class TelegramSender(ITelegramBotClient botClient) : ITelegramSender
{
    public async Task SendTextAsync(long chatId, string text, CancellationToken ct = default) =>
        await botClient.SendMessage(chatId, text, cancellationToken: ct);
}
