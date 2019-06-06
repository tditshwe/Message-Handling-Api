using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateSent { get; set; }
        public int GroupId  { get; set; }

        [ForeignKey("Account")]
        public string Sender { get; set; }

        [ForeignKey("Account")]
        public string Receiver { get; set; }
    }
}