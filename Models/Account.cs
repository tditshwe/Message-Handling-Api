namespace MessageHandlingApi.Models
{
    public class Account
    {
        public int Id;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}