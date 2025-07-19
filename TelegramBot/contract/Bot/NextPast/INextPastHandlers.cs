using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.NextPast
{
    public interface INextPastHandlers : ITelegramUpdateHandler
    {
        Task HandleUserPageAsync(Update update, CancellationToken cancellationToken);
    }
}
