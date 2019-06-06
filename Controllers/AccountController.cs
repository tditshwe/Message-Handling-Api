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
    public class AccountController : ControllerBase
    {
        private readonly MessageContext Context = new MessageContext();

        /*public AccountController(MessageContext ctx)
        {
            Context = ctx;
        }*/ 
                
        /// <summary>
        /// Create a new account
        /// </summary>
        // POST api/values
        [AllowAnonymous]
        [HttpPost]
        public void Create([FromBody] AccountBody acc)
        {
            PasswordHasher<Account> hasher = new PasswordHasher<Account>();

            Account newAcc = new Account
            {
                Username = acc.Username,
                Role = "User",
                Status = "Ready to chat"
            };

            // Hash account password
            string hashed = hasher.HashPassword(newAcc, acc.Password);           

            newAcc.Password = hashed;         

            Context.Account.Add(newAcc);
            Context.SaveChanges();
        }
    }
}