using MongoDB.Bson.Serialization.Attributes;
using TelegramBot.model.attribute;
using TelegramBot.model.Base;

namespace TelegramBot.model.state
{
    [CollectionName("userstats")]
    public class UserState
    {
        [BsonId]
        public long ChatId { get; set; } 

        public string State { get; set; } = "None";
        public string MotherState { get; set; }

        public Dictionary<string, string> Data { get; set; } = new();
    }

}
