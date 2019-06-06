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
        // POST messageHandlingApi/Account
        [AllowAnonymous]
        [HttpPost]
        public void Create([FromBody] AccountCreate acc)
        {
            PasswordHasher<Account> hasher = new PasswordHasher<Account>();

            Account newAcc = new Account
            {
                Username = acc.Username,
                Role = "User",
                Status = "Ready to chat",
                Name = acc.Name
            };

            // Hash account password
            string hashed = hasher.HashPassword(newAcc, acc.Password);           

            newAcc.Password = hashed;         

            Context.Account.Add(newAcc);
            Context.SaveChanges();
        }

        /// <summary>
        /// Authenticate account and obtain user token
        /// </summary>
        // POST messageHandlingApi/Account/Authenticate
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody] AccountLogin login)
        {   
            PasswordHasher<Account> hasher = new PasswordHasher<Account>();  
            var account = Context.Account.SingleOrDefault(x => x.Username == login.Username && hasher.VerifyHashedPassword(x, x.Password, login.Password) == PasswordVerificationResult.Success);

            // return null if account is not found
            if (account == null)
               return BadRequest(new { message = "Invalid login details" });

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Secret for generating JWT tokens
            string secret = "WhatsApp Messenger";
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.Role, account.Role)
                }),
                //Token expires after 7 day days
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            account.Token = tokenHandler.WriteToken(token);

            return Ok (tokenHandler.WriteToken(token));
        }

        /// <summary>
        /// Update authenticated user account
        /// </summary>
        // POST messageHandlingApi/Account
        [HttpPut]
        public void Edit([FromBody] Account acc)
        {
            var username = User.Identity.Name;
            Account edited = Context.Account.Find(username);

            edited.Name = acc.Name;
            edited.Status = acc.Status;

            Context.Account.Update(edited);
            Context.SaveChanges();
        }
    }     
}