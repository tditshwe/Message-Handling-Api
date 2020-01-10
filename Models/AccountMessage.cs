namespace MessageHandlingApi.Models
{
    public class AccountMessage
    {
        public string AccountUsername { get; set; }
        public Account Account { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}