using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TelegramBot.model.Base
{
    public class Base_E
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
    }

}
