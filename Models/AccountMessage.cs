namespace MessageHandlingApi.Models
{
    public class AccountMessage
    {
        public string AccountUsername { get; set; }
        public virtual Account Account { get; set; }
        public int MessageId { get; set; }
        public virtual Message Message { get; set; }
    }
}