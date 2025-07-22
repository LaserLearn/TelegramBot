using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.group
{
    public interface IGroupHandler : ITelegramUpdateHandler
    {
        Task HandleWelcomeAsync(Update update, CancellationToken cancellationToken);
        Task HandleLinkLockAsync(Update update, CancellationToken cancellationToken);
        Task HandleVoiceLockAsync(Update update, CancellationToken cancellationToken);
        Task HandleModerationAsync(Update update, CancellationToken cancellationToken); // برای اخراج، سکوت، هشدار
        Task HandleAutoPinAsync(Update update, CancellationToken cancellationToken);
        Task HandleAutoReplyAsync(Update update, CancellationToken cancellationToken);
    }
}
