using MongoDB.Driver;
using TelegramBot.config.mongo;
using TelegramBot.contract.Database.user;
using TelegramBot.Imp.Database.generic;
using TelegramBot.model.user;

namespace TelegramBot.Imp.Database.user
{
    public class User_Rep : GenericRepository<User_E>, IUser_Rep
    {
        public User_Rep(IMongoClient mongoClient, MongoDBSettings settings)
               : base(mongoClient, settings) 
        {
        }
        public async Task<User_E?> GetBy_Email(string email)
        {
            return await GetCollection().Find(u => u.email == email).FirstOrDefaultAsync();
        }

        public async Task<User_E?> GetBy_TelegramId(long telegram_id)
        {
            return await GetCollection().Find(u => u.telegram_id == telegram_id).FirstOrDefaultAsync();
        }
    }
}
