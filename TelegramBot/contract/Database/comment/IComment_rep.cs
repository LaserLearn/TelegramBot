using MongoDB.Driver;
using TelegramBot.config.mongo;
using TelegramBot.contract.Database.generic;
using TelegramBot.Imp.Database.generic;
using TelegramBot.model.comment;

namespace TelegramBot.contract.Database.comment
{
    public interface IComment_rep : IGenericRepository<Comment_E>
    {
    }
}
