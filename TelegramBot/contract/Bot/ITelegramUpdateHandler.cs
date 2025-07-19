using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot
{
    public interface ITelegramUpdateHandler
    {
        Task DispatchAsync(Update update, CancellationToken cancellationToken);
        Task<bool> ShouldHandleAsync(Update update);
    }

}
