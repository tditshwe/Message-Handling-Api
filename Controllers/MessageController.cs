using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using MessageHandlingApi.Models;
using Microsoft.AspNetCore.Identity;

namespace MessageHandlingApi.Controllers
{
    [Authorize]
    [Route("messageHandlingApi/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageContext Context = new MessageContext();

        /// <summary>
        /// Get chat history with specific contact
        /// </summary>
        // POST messageHandlingApi/Message/{contact}
        [HttpGet ("{contact}")]
        public IActionResult ChatHistory(string contact)
        {
            var username = User.Identity.Name;
            var chat = Context.Message.Where(m => m.Sender == username && m.Receiver == contact).ToList();
            List<MessageRetrieve> chatList = new List<MessageRetrieve>();

            chat.ForEach(
                c => chatList.Add(new MessageRetrieve
                {
                    Sender = c.Sender,
                    SenderName = Context.Account.Find(c.Sender).Name,
                    DateSent = c.DateSent,
                    Text = c.Text
                })
            );

            return Ok (chatList);
        }

        /// <summary>
        /// Get group chat
        /// </summary>
        // POST messageHandlingApi/Message/groupChat/{groupId}
        [HttpGet ("groupChat/{groupId}")]
        public IActionResult GroupChat(int groupId)
        {
            var username = User.Identity.Name;
            var chat = Context.Message.Where(m => m.Sender == username && m.GroupsId == groupId).ToList();
            List<MessageRetrieve> chatList = new List<MessageRetrieve>();

            chat.ForEach(
                c => chatList.Add(new MessageRetrieve
                {
                    Sender = c.Sender,
                    SenderName = Context.Account.Find(c.Sender).Name,
                    DateSent = c.DateSent,
                    Text = c.Text
                })
            );

            return Ok (chatList);
        }

        /// <summary>
        /// Send message to contact
        /// </summary>
        // POST messageHandlingApi/Message/{contact}/{text}
        [HttpPost ("{contact}/{text}")]
        public IActionResult SendToContact(string contact, string text)
        {
            if (Context.Account.Find(contact) == null)
                return BadRequest(new { message = "Invalid contact" });

            Message msg = new Message
            {
                Text = text,
                DateSent = DateTime.Now,
                Sender =  User.Identity.Name,
                Receiver = contact,
                GroupsId = 1
            };

            Context.Message.Add(msg);
            Context.SaveChanges();

            return Ok("Created");
        }

        /// <summary>
        /// Send message to group
        /// </summary>
        // POST messageHandlingApi/Message/{groupId}/{text}
        [HttpPost ("sendToGroup/{groupId}/{text}")]
        public IActionResult SendToGroup(int groupId, string text)
        {
            Groups group = Context.Groups.Find(groupId);
            var account = Context.Account.Find(User.Identity.Name);

            if (group == null)
                return BadRequest(new { message = "Invalid group" });

            //if (!group.Participants.Contains(account))
                //return BadRequest(new { message = "You are not the member of this group" });

            Message msg = new Message
            {
                Text = text,
                DateSent = DateTime.Now,
                Sender =  User.Identity.Name,
                Receiver = User.Identity.Name,
                GroupsId = groupId
            };

            Context.Message.Add(msg);
            Context.SaveChanges();

            return Ok("Created");
        }
    }
}