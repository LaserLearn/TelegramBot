namespace TelegramBot.model.attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionNameAttribute : Attribute
    {
        public string TableName { get; }

        public CollectionNameAttribute(string name)
        {
            TableName = name;
        }
    }
}
