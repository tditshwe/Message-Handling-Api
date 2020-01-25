namespace MessageHandlingApi.Models
{
    public class GroupMessage {
        public int MessageId { get; set; }
        public virtual Message Message { get; set; }
        public int GroupId { get; set; }
        public virtual Groups Group { get; set; }
    }
}