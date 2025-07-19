using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.channel
{
    public interface IChannelHandler : ITelegramUpdateHandler
    {
        Task<bool> HandleSendChannelVideoAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandleSendChannelMessageAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandleSendChannelImageAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandleSendChannelPollAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandlShowMenuAsync(Update update, CancellationToken cancellationToken);
    }

}
