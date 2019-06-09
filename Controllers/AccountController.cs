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

        /// <summary>
        /// Get account info
        /// </summary>
        // GET messageHandlingApi/Account/
        [HttpGet]
        public IActionResult GetAccount()
        {
            var acc = Context.Account.Find(User.Identity.Name);

            return Ok (new AccountRetrieve
            {
                Username = acc.Username,
                Name = acc.Name,
                Status = acc.Status,
                Role = acc.Role,
                ImageUrl = acc.ImageUrl
            });
        }

        /// <summary>
        /// Get a list of all accounts
        /// </summary>
        // GET messageHandlingApi/Account/AccountList
        [HttpGet ("AccountList")]
        public IActionResult GetAccountList()
        {
            var accounts = Context.Account.ToList();
            List<AccountRetrieve> accList = new List<AccountRetrieve>();

            accounts.ForEach(
                ac => accList.Add(new AccountRetrieve
                {
                    Username = ac.Username,
                    Name = ac.Name,
                    Status = ac.Status,
                    Role = ac.Role,
                    ImageUrl = ac.ImageUrl
                })
            );

            return Ok (accList);
        }
                
        /// <summary>
        /// Create a new account
        /// </summary>
        // POST messageHandlingApi/Account
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromBody] AccountCreate acc)
        {
            var existing = Context.Account.Find(acc.Username);

            if (existing != null)
                return BadRequest("This username is already taken by another person");

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

            return Ok();
        }

        /// <summary>
        /// Login to obtain user token
        /// </summary>
        // POST messageHandlingApi/Account/Login
        [AllowAnonymous]
        [HttpPost("Login")]
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
            return Ok (tokenHandler.WriteToken(token));
        }

        /// <summary>
        /// Update authenticated user account
        /// </summary>
        // PUT messageHandlingApi/Account
        [HttpPut]
        public void Edit([FromBody] AccountEdit acc)
        {
            var username = User.Identity.Name;
            Account edited = Context.Account.Find(username);

            edited.Name = acc.Name;
            edited.Status = acc.Status;

            Context.Account.Update(edited);
            Context.SaveChanges();
        }

        /// <summary>
        /// Delete your account
        /// </summary>
        // PUT messageHandlingApi/Account
        [HttpDelete]
        public void Delete()
        {
            var account = Context.Account.Find(User.Identity.Name);

            Context.Account.Remove(account);
            Context.SaveChanges();
        }
    }     
}