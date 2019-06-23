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

            if (contact == User.Identity.Name)
                return BadRequest("You can't chat with yourself");

            var chat = Context.Message.Where(m => (m.Sender == username && m.Receiver == contact) || (m.Sender == contact && m.Receiver == username)).ToList();
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

            if (contact == User.Identity.Name)
                return BadRequest("You can't send a message to yourself");

            var chat = Context.Chat.Where(c => (c.SenderUsername == User.Identity.Name && c.ReceiverUsername == contact)
                || ( c.ReceiverUsername == User.Identity.Name && c.SenderUsername == contact)).First();

            chat.LastText = text;
            chat.LastMessageDate = DateTime.Now;

            Message msg = new Message
            {
                Text = text,
                DateSent = DateTime.Now,
                Sender =  User.Identity.Name,
                Receiver = contact,
                GroupsId = 1
            };

            Context.Message.Add(msg);
            Context.Chat.Update(chat);
            Context.SaveChanges();

            return Ok();
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
            var accGroup = Context.AccountGroup.Where(g => g.GroupId == groupId).ToList();

            var link = new AccountGroup
            {
                Username = User.Identity.Name,
                GroupId = groupId
            };

            if (group == null)
                return BadRequest(new { message = "Invalid group" });

            if (!accGroup.Contains(link))
                return BadRequest(new { message = "You are not the member of this group" });

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

        /// <summary>
        /// Delete a single message
        /// </summary>
        // DELETE messageHandlingApi/Message/{id}
        [HttpDelete ("{id}")]
        public IActionResult Delete(int id)
        {
            var message = Context.Message.Find(id);

            if (message == null)
                return NotFound("Message doesn't exist");

            Context.Message.Remove(message);
            Context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Delete chat history
        /// </summary>
        // DELETE messageHandlingApi/Message/deleteChat/{contact}
        [HttpDelete ("deleteChat/{contact}")]
        public void DeleteChat(string contact)
        {
            var username = User.Identity.Name;
            var chat = Context.Message.Where(m => m.Sender == username && m.Receiver == contact).ToList();

            chat.ForEach(
                c =>  Context.Message.Remove(c)
            );
          
            Context.SaveChanges();
        }
    }
}