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
            try
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Get a list of all accounts
        /// </summary>
        // GET messageHandlingApi/Account/AccountList
        [HttpGet ("AccountList")]
        public IActionResult GetAccountList()
        {
            try
            {
                var accounts = Context.Account.Where(ac => ac.Username != User.Identity.Name).ToList();
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
                
        /// <summary>
        /// Create a new account
        /// </summary>
        // POST messageHandlingApi/Account
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create([FromBody] AccountCreate acc)
        {
            try
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Login to obtain user token
        /// </summary>
        // POST messageHandlingApi/Account/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Authenticate([FromBody] AccountLogin login)
        {
            try
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        /// <summary>
        /// Upload profile picture
        /// </summary>
        // POST messageHandlingApi/Account/imgUpload
        [HttpPost ("imgUpload")]
        public IActionResult UploadImage()
        {
            try
            {
                var file = Request.Form.Files[0];
                string folderName = "Profile-pictures";
                string newPath = Path.Combine(folderName);

                // Create folder if it doesn't exist 
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)  
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        // Max size = 1 MB
                        int maxContentLength = 1024 * 1024 * 1;
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };  
                        var ext = fileName.Substring(fileName.LastIndexOf('.'));  
                        var extension = ext.ToLower();

                        if (!AllowedFileExtensions.Contains(extension))    
                            return BadRequest("Please Upload image of type .jpg,.gif,.png.");  

                        if (file.Length > maxContentLength)  
                            return BadRequest("Please Upload a file upto 1 MB.");   
                        
                        file.CopyTo(stream);

                        Account acc = Context.Account.Find(User.Identity.Name);
                        acc.ImageUrl = fullPath;
                        Context.Account.Update(acc);
                        Context.SaveChanges();
                    }  

                    return Ok ("Profile picture uploaded successfully.");   
                }

                return StatusCode(404, "Please upload an image");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Update authenticated user account
        /// </summary>
        // PUT messageHandlingApi/Account
        [HttpPut]
        public IActionResult Edit([FromBody] AccountEdit acc)
        {
            try
            {
                var username = User.Identity.Name;
                Account edited = Context.Account.Find(username);

                edited.Name = acc.Name;
                edited.Status = acc.Status;

                Context.Account.Update(edited);
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Delete your account
        /// </summary>
        // PUT messageHandlingApi/Account
        [HttpDelete]
        public IActionResult Delete()
        {
            try
            {
                var account = Context.Account.Find(User.Identity.Name);
                var chat = Context.Message.Where(m => m.Sender == account.Username || m.Receiver == account.Username).ToList();
                var accountGroups = Context.Groups.Where(g => g.CreatorUsername == User.Identity.Name);

                // Delete all messages sent and received by account
                chat.ForEach(
                    c => Context.Message.Remove(c)
                );

                // Iterate through all groups created by the account to be deleted
                foreach (Groups gr in accountGroups)
                {
                    var accGroup = Context.AccountGroup.Where(g => g.GroupId == gr.Id).ToList();
                    
                    // If group has members
                    if (accGroup.Count() > 0)
                    {
                        // change the group creator to the first member
                        var acc = Context.Account.Find(accGroup[0].Username);
                        gr.CreatorUsername = accGroup[0].Username;
                        acc.Role = "GroupAdmin";

                        Context.Account.Update(acc);
                        Context.Groups.Update(gr);
                    }
                    else
                        Context.Groups.Remove(gr);
                }

                Context.SaveChanges();
                Context.Account.Remove(account);
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