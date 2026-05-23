using System.Text.Json;
using StackExchange.Redis;

namespace TelegramBotService.Session;

public sealed class SessionService(IConnectionMultiplexer redis)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);
    private IDatabase Db => redis.GetDatabase();

    private static string Key(long chatId) => $"bot:session:{chatId}";

    public async Task<BotSession> GetOrCreateAsync(long chatId)
    {
        var value = await Db.StringGetAsync(Key(chatId));
        if (!value.HasValue)
            return new BotSession();

        return JsonSerializer.Deserialize<BotSession>(value!) ?? new BotSession();
    }

    public async Task SaveAsync(long chatId, BotSession session) =>
        await Db.StringSetAsync(Key(chatId),
            JsonSerializer.Serialize(session), Ttl);

    public async Task ClearAsync(long chatId) =>
        await Db.KeyDeleteAsync(Key(chatId));
}
