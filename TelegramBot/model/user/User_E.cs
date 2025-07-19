using System.Reflection.Metadata.Ecma335;
using TelegramBot.model.attribute;
using TelegramBot.model.Base;
using TelegramBot.model.Enum;

namespace TelegramBot.model.user
{
    [CollectionName("users")]
    public class User_E : Base_E
    {
        public string user_name { get; set; } = null!;
        public long? telegram_id { get; set; } 
        public string email { get; set; } = null!;
        public int age { get; set; }
        public Role_Em role_Em { get; set; }
    }
}
