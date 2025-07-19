using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.Toggele
{
    public interface IToggleHandlers : ITelegramUpdateHandler
    {
        Task SendInitialToggleMessage(Update update, CancellationToken cancellationToken);
    }
}
