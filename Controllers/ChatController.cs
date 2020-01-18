using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ChatController : ControllerBase
    {
        private readonly MessageContext Context = new MessageContext();

        /// <summary>
        /// Get a list of user chats
        /// </summary>
        // GET messageHandlingApi/Chat/
        [HttpGet]
        public IActionResult ChatList()
        {
            var chats = Context.Chat           
                .Where(c => c.SenderUsername == User.Identity.Name || c.ReceiverUsername == User.Identity.Name)
                //.Include(c => c.Sender)
                //.Include(c => c.Receiver)
                .ToList();
            
            List<Chat> chatList = new List<Chat>();

            chats.ForEach(c => {
                chatList.Add(new Chat {
                    Id = c.Id,
                    ReceiverUsername = c.ReceiverUsername,
                    SenderUsername = c.SenderUsername,
                    Receiver = new Account {
                        Username = c.ReceiverUsername,
                        Name = c.Receiver.Name,
                        Status = c.Receiver.Status,
                        ImageUrl = c.Receiver.ImageUrl
                    },
                    Sender = new Account {
                        Username = c.SenderUsername,
                        Name = c.Sender.Name,
                        Status = c.Sender.Status,
                        ImageUrl = c.Sender.ImageUrl
                    },
                    LastMessage = new Message {
                        Id = c.LastMessageId,
                        Text = c.LastMessage.Text,
                        DateSent = c.LastMessage.DateSent
                    }
                });
            });

            return Ok(chatList);
        }

        /// <summary>
        /// Create new chat
        /// </summary>
        // POST messageHandlingApi/Chat
        [HttpPost]
        public void Create(string username, int groupId)
        {
            /*Chat newChat = new Chat
            {
                LastText = "",
                LastMessageDate = DateTime.Now,
                SenderUsername = User.Identity.Name,                
            };

            if (username == null)
            {
                newChat.IsGroup = true;
                newChat.GroupId = groupId;
            }
            else
            {
                newChat.ReceiverUsername = username;
            }

            Context.Chat.Add(newChat);*/
            Context.SaveChanges();
        }
    }
}