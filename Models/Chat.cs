using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string LastText { get; set; } 
        public DateTime LastMessageDate { get; set; }
        public bool IsGroup { get; set; }
        public string ReceiverUsername { get; set; }
        public int GroupId { get; set; }
        public string SenderUsername { get; set; }
        public Account Receiver { get; set; }
    }
}