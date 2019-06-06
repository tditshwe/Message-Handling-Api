using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class Groups
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Account> Participants { get; set; }
    }
}