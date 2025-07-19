using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.start
{
    public interface IStartHandler : ITelegramUpdateHandler
    {
        Task HandleStartAsync(Update update, CancellationToken cancellationToken);
    }

}
