using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageHandlingApi.Models
{
    public class GroupInfo
    {
        public string Name { get; set; }
        public AccountEdit Creator { get; set; }
        public int Participants { get; set; }
        public List<AccountEdit> ListOfParticipants { get; set; }
    }
    public class Groups
    {
        public int Id { get; set; }
        public string Name { get; set; }       
        public string CreatorUsername { get; set; }
        public virtual IList<AccountGroup> GroupAccounts { get; set; }
        public virtual IList<GroupMessage> GroupMessages { get; set; }
    }
}