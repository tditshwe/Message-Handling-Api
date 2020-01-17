namespace MessageHandlingApi.Models
{   
    public class AccountGroup
    {
        public string AccountUsername { get; set; }
        public virtual Account Account { get; set; }
        public int GroupId { get; set; }
        public virtual Groups Group { get; set; }
    }
}