using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.selenium
{
    public interface IDigikalaHandler : ITelegramUpdateHandler
    {
        Task HandleProduct(Update update, CancellationToken cancellationToken);
    }
}
