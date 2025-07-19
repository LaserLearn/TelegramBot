using MongoDB.Driver;
using TelegramBot.config.mongo;
using TelegramBot.contract.Database.comment;
using TelegramBot.Imp.Database.generic;
using TelegramBot.model.comment;

namespace TelegramBot.Imp.Database.comment
{
    public class Comment_rep : GenericRepository<Comment_E>, IComment_rep
    {
        public Comment_rep(IMongoClient mongoClient, MongoDBSettings settings)
            : base(mongoClient, settings) { }
    }

}
