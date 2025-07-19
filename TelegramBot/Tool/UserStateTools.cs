using MongoDB.Driver;
using Telegram.Bot.Types;
using TelegramBot.model.state;

namespace TelegramBot.Tool
{
    public static class UserStateTools
    {
        private static IMongoCollection<UserState> _collection;

        public static void Initialize(IMongoDatabase database)
        {
            _collection = database.GetCollection<UserState>("userstats");
        }

        public static async Task SetStateAsync(long? chatId, string state)
        {
            var update = Builders<UserState>.Update.Set(u => u.State, state);
            await _collection.UpdateOneAsync(
                u => u.ChatId == chatId,
                update,
                new UpdateOptions { IsUpsert = true });
        }

        public static async Task SetMotherAsync(long? chatId, string state)
        {
            var update = Builders<UserState>.Update.Set(u => u.MotherState, state);
            await _collection.UpdateOneAsync(
                u => u.ChatId == chatId,
                update,
                new UpdateOptions { IsUpsert = true });
        }
        public static long? GetChatId(Update update)
        {
            long? chatId = null;

            if (update.Message != null)
            {
                chatId = update.Message.Chat.Id;
            }
            else if (update.CallbackQuery != null)
            {
                chatId = update.CallbackQuery.Message?.Chat.Id;
            }
            return chatId;
        }

        public static async Task<string?> GetStateAsync(long chatId)
        {
            var user = await _collection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            return user?.State;
        }
        public static async Task<string?> GetMotherStateAsync(long? chatId)
        {
            var user = await _collection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            return user?.MotherState;
        }

        public static async Task SetDataAsync(long? chatId, string key, string value)
        {
            var update = Builders<UserState>.Update.Set($"Data.{key}", value);
            await _collection.UpdateOneAsync(
                u => u.ChatId == chatId,
                update,
                new UpdateOptions { IsUpsert = true });
        }

        public static async Task<string?> GetDataAsync(long chatId, string key)
        {
            var user = await _collection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (user != null && user.Data.TryGetValue(key, out var value))
                return value;

            return null;
        }

        public static async Task DeleteDataAsync(long chatId, string key)
        {
            var update = Builders<UserState>.Update.Unset($"Data.{key}");
            await _collection.UpdateOneAsync(u => u.ChatId == chatId, update);
        }

        public static async Task ClearAsync(long? chatId)
        {
            await _collection.DeleteOneAsync(u => u.ChatId == chatId);
        }

        public static async Task<UserState?> GetAllAsync(long? chatId)
        {
            return await _collection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
        }

    }

}
