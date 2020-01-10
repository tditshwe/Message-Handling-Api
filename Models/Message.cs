using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class MessageRetrieve
    {
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public DateTime DateSent { get; set; }
        public string Text { get; set; }
    }
    
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateSent { get; set; }
        public string SenderUsername { get; set; }
        //public virtual Account Sender { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual IList<AccountMessage> AccountMessages { get; set; }
    }
}