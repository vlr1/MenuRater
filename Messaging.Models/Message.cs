namespace Messaging.Models
{
    public class Message
    {
        public string CommandName { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public Type Type { get; set; } = typeof(object);
    }
}