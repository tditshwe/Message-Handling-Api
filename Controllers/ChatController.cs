using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
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
            var chats = Context.Chat.Where(c => c.SenderUsername == User.Identity.Name || c.ReceiverUsername == User.Identity.Name).ToList();

            foreach (Chat c in chats)
            {
                Account rec;

                if (c.SenderUsername == User.Identity.Name)
                    rec =  Context.Account.Find(c.ReceiverUsername);
                else
                    rec =  Context.Account.Find(c.SenderUsername);

                c.Receiver = new Account
                { 
                    Username = rec.Username,
                    Name = rec.Name,
                    ImageUrl = rec.ImageUrl
                };
            }

            return Ok(chats);
        }

        /// <summary>
        /// Create new chat
        /// </summary>
        // POST messageHandlingApi/Chat
        [HttpPost]
        public void Create(string username, int groupId)
        {
            Chat newChat = new Chat
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
                newChat.IsGroup = false;
                newChat.ReceiverUsername = username;
            }

            Context.Chat.Add(newChat);
            Context.SaveChanges();
        }
    }
}