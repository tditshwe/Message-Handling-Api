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

    public class AccountEdit
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class AccountRetrieve
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string ImageUrl  { get; set; }
    }
    
    public class Account
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string ImageUrl  { get; set; }

        public virtual ICollection<Chat> ChatsSent  { get; set; }
        public virtual ICollection<Chat> ChatsReceived  { get; set; }
        //public virtual ICollection<Groups> GroupsCreated  { get; set; }
        public virtual IList<AccountMessage> AccountMessages { get; set; }
        public virtual IList<AccountGroup> AccountGroups { get; set; }
    }
}