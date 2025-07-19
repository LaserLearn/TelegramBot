using TelegramBot.contract.Database.generic;
using TelegramBot.model.user;

namespace TelegramBot.contract.Database.user
{
    public interface IUser_Rep : IGenericRepository<User_E>
    {
        Task<User_E?> GetBy_TelegramId(long telegram_id);
        Task<User_E?> GetBy_Email(string email);
    }
}
