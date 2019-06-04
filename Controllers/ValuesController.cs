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
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [AllowAnonymous]
        [HttpPost("AuthenticateAccount")]
        public IActionResult Authenticate([FromBody] Account acc)
        {
            List<Account> _users = new List<Account>
            { 
                new Account { Username = "test", Password = "AQAAAAEAACcQAAAAENpksZoNlEQuaqB1jqucxHM5hFGYjYqtslvuBzxn3VbbbCxNB3gvKCUgipxHxVKh7A==", Role = "User" },
                new Account { Username = "admin", Password = "AQAAAAEAACcQAAAAEDbBKQo/fKTlpqYA/1/+Maf1f1UeQ+5acJYIUw38VkAJL/nF4FQ3gS2SAGIGwaZ2Iw==", Role = "GroupAdmin" } 
            };

            PasswordHasher<Account> hasher = new PasswordHasher<Account>();      

            var user = _users.SingleOrDefault(x => x.Username == acc.Username && hasher.VerifyHashedPassword(x, x.Password, acc.Password) == PasswordVerificationResult.Success);

            // return null if user not found
            if (user == null)
               return BadRequest(new { message = "Wrong login details" });

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Secret for generating JWT tokens
            string secret = "WhatsApp Messenger";
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                //Token expires after a day
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return Ok (user);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [Authorize(Roles = "GroupAdmin")]
        [HttpPut("{id}")]
        public ActionResult Put(int id)
        {
            var username = User.Identity.Name;
            return Ok(string.Format("Value {0} updated by {1}", id, username)); 
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
