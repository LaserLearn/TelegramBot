using Telegram.Bot.Types;

namespace TelegramBot.contract.Bot.register
{
    public interface IRegisterUserHandler : ITelegramUpdateHandler
    {
        Task<bool> HandleReceiveNameAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandleReceiveEmailAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandleReceiveAgeAsync(Update update, CancellationToken cancellationToken);
        Task<bool> HandleReceiveRoleAsync(Update update, CancellationToken cancellationToken);
        Task<bool> ShowRoleMenuAsync(Update update, CancellationToken cancellationToken);
        Task<bool> SaveUserAsync(Update update, CancellationToken cancellationToken);
        Task<bool> ShowUserInfoAsync(Update update, CancellationToken cancellationToken);
        Task<bool> CheckUserExistsAsync(Update update, CancellationToken cancellationToken);
    }

}
