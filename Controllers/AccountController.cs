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
        // POST api/values
        /// <summary>
        /// Create a new account
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public void Create([FromBody] Account acc)
        {
            PasswordHasher<Account> hasher = new PasswordHasher<Account>();

            // Hash account password
            string hashed = hasher.HashPassword(acc, acc.Password);

            acc.Password = hashed;
            acc.Role = "User";
        }
    }
}