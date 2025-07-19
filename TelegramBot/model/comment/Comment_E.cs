using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using TelegramBot.model.attribute;
using TelegramBot.model.Base;

namespace TelegramBot.model.comment
{
    [CollectionNameAttribute("comments")]
    public class Comment_E : Base_E
    {
        public string? content { get; set; }
        public float score { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string document_id { get; set; } = null!;
    }

}
