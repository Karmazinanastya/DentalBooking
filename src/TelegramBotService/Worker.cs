using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBotService.Handlers;

namespace TelegramBotService;

public sealed class Worker(
    ITelegramBotClient bot,
    IServiceProvider services,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var offset = 0;

        logger.LogInformation("Telegram bot started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var updates = await bot.GetUpdates(
                    offset,
                    allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
                    timeout: 30,
                    cancellationToken: stoppingToken);

                foreach (var update in updates)
                {
                    await using var scope = services.CreateAsyncScope();
                    var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
                    await handler.HandleAsync(update, stoppingToken);
                    offset = update.Id + 1;
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during polling. Retrying in 5s.");
                await Task.Delay(5000, stoppingToken);
            }
        }

        logger.LogInformation("Telegram bot stopped.");
    }
}
