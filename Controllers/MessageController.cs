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

                List<Message> chatMessages = new List<Message>();

                var contactMessages = Context.Account.Find(contact)
                    .AccountMessages
                    .Where(am => am.Message.SenderUsername == User.Identity.Name);

                var messages = Context.Account.Find(User.Identity.Name)
                    .AccountMessages
                    .Where(am => am.Message.SenderUsername == contact)
                    .Union(contactMessages)
                    .OrderBy(am => am.Message.DateSent)
                    .ToList();

                messages.ForEach(m => chatMessages.Add(new Message {
                    Id = m.Message.Id,
                    Text = m.Message.Text,
                    DateSent = m.Message.DateSent,
                    SenderUsername = m.Message.SenderUsername,
                    Sender = new Account {
                        Name = m.Message.Sender.Name
                    }
                }));

                return Ok (chatMessages);
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
                var groupMsg = Context.Groups.Find(groupId).GroupMessages.ToList();
                List<Message> msgList = new List<Message>();

                groupMsg.ForEach(
                    m => msgList.Add(new Message
                    {
                        Id = m.Message.Id,
                        Text = m.Message.Text,
                        DateSent = m.Message.DateSent,
                        SenderUsername = m.Message.SenderUsername,
                        Sender = new Account {
                            Name = m.Message.Sender.Name
                        }
                    })
                );

                return Ok (msgList);
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
                    || ( c.ReceiverUsername == User.Identity.Name && c.SenderUsername == contact));

                Message msg = new Message
                {
                    Text = text,
                    DateSent = DateTime.Now,
                    SenderUsername =  User.Identity.Name,
                };

                Context.Message.Add(msg);
                Context.SaveChanges();

                AccountMessage AccMess = new AccountMessage {
                    AccountUsername = contact,
                    MessageId = msg.Id
                };
                
                Context.AccountMessage.Add(AccMess);                

                if (chat.Count() == 0)
                {
                    Chat newChat = new Chat
                    {
                        SenderUsername = User.Identity.Name,
                        ReceiverUsername = contact,
                        LastMessageId = msg.Id           
                    };

                    Context.Chat.Add(newChat);
                }
                else
                {
                    Chat c = chat.First();
                    c.LastMessageId = msg.Id;
                    Context.Chat.Update(c);
                }

                if (contact == User.Identity.Name)
                    return BadRequest("You can't send a message to yourself");

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

                if (group == null)
                    return BadRequest(new { message = "Invalid group" });

                Message msg = new Message
                {
                    Text = text,
                    DateSent = DateTime.Now,
                    SenderUsername =  User.Identity.Name,
                };

                Context.Message.Add(msg);
                Context.SaveChanges();

                var link = new GroupMessage
                {
                    GroupId = groupId,
                    MessageId = msg.Id
                };

                //if (!accGroup.Contains(link))
                    //return BadRequest(new { message = "You are not the member of this group" });

                Context.GroupMessage.Add(link);
                Context.SaveChanges();

                return Ok();
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
                
                if (msg.SenderUsername != User.Identity.Name)
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

                if (message.SenderUsername != User.Identity.Name)
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
                var chat = Context.Message.Where(m => m.SenderUsername == username).ToList();

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