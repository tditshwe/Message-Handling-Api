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
        // GET messageHandlingApi/Message/{contact}
        [HttpGet ("{contact}")]
        public IActionResult ChatHistory(string contact)
        {
            try
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Get group chat
        /// </summary>
        // GET messageHandlingApi/Message/groupChat/{groupId}
        [HttpGet ("groupChat/{groupId}")]
        public IActionResult GroupChat(int groupId)
        {
            try
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Send message to contact
        /// </summary>
        // POST messageHandlingApi/Message/{contact}/{text}
        [HttpPost ("{contact}/{text}")]
        public IActionResult SendToContact(string contact, string text)
        {
            try
            {
                if (Context.Account.Find(contact) == null)
                    return BadRequest(new { message = "Invalid contact" });

                var chat = Context.Chat.Where(c => (c.SenderUsername == User.Identity.Name && c.ReceiverUsername == contact)
                    || ( c.ReceiverUsername == User.Identity.Name && c.SenderUsername == contact)).First();

                if (chat == null)
                {
                    Chat newChat = new Chat
                    {
                        LastText = text,
                        LastMessageDate = DateTime.Now,
                        SenderUsername = User.Identity.Name,
                        IsGroup = false,
                        ReceiverUsername = contact             
                    };

                    Context.Chat.Add(newChat);
                }
                else
                {
                    chat.LastText = text;
                    chat.LastMessageDate = DateTime.Now;
                    Context.Chat.Update(chat);
                }

                if (contact == User.Identity.Name)
                    return BadRequest("You can't send a message to yourself");

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

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Send message to group
        /// </summary>
        // POST messageHandlingApi/Message/{groupId}/{text}
        [HttpPost ("sendToGroup/{groupId}/{text}")]
        public IActionResult SendToGroup(int groupId, string text)
        {
            try
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Edit message
        /// </summary>
        // PUT messageHandlingApi/Message/{id}/{newText}
        [HttpPut ("{id}/{newText}")]
        public ActionResult Edit(int id, string newText)
        {
            try
            {
                var msg = Context.Message.Find(id);

                if (msg == null)
                    return StatusCode(404, "Message not found");
                
                if (msg.Sender != User.Identity.Name)
                    return BadRequest("You did not write this message");

                msg.Text = newText;
                Context.Message.Update(msg);
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Delete a single message
        /// </summary>
        // DELETE messageHandlingApi/Message/{id}
        [HttpDelete ("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var message = Context.Message.Find(id);

                if (message == null)
                    return NotFound("Message doesn't exist");

                if (message.Sender != User.Identity.Name)
                    return BadRequest("You are not the sender of this message");

                Context.Message.Remove(message);
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Delete chat history
        /// </summary>
        // DELETE messageHandlingApi/Message/deleteChat/{contact}
        [HttpDelete ("deleteChat/{contact}")]
        public IActionResult DeleteChat(string contact)
        {
            try
            {
                var username = User.Identity.Name;
                var chat = Context.Message.Where(m => m.Sender == username && m.Receiver == contact).ToList();

                chat.ForEach(
                    c =>  Context.Message.Remove(c)
                );
            
                Context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}