using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class Account
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }

        //[NotMapped]
        public string Token { get; set; }
        public string Role { get; set; }
        public string ImageUrl  { get; set; }
        public int GroupId  { get; set; }
        //public virtual ICollection<Message> Messages  { get; set; }
    }
}