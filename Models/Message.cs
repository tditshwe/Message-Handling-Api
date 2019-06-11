using System;
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
        public int GroupsId  { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}