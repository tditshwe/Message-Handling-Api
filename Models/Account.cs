using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class AccountLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AccountCreate
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
    
    public class Account
    {
        /*public Account()
        {
            Groups = new HashSet<Groups>();
        }*/

        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public string Token { get; set; }
        public string Role { get; set; }
        public string ImageUrl  { get; set; }
        //public int GroupsId  { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Groups> Groups { get; set; }
    }
}