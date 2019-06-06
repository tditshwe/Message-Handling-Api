using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class AccountBody
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Account
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public string Token { get; set; }
        public string Role { get; set; }
        public string ImageUrl  { get; set; }
        public int GroupsId  { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}