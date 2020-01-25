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
    public class GroupController : ControllerBase
    {
        private readonly MessageContext Context = new MessageContext();

        /// <summary>
        /// Get a list of group participants
        /// </summary>

        // GET messageHandlingApi/Group/{id}}
        [HttpGet ("participants/{id}")]
        public IActionResult Participants(int id)
        {
            try
            {
                var group = Context.Groups.Find(id);
                var groupAccounts = group.GroupAccounts.ToList();
                List<Account> participants = new List<Account>();

                groupAccounts.ForEach(g => participants.Add(new Account {
                    Username = g.AccountUsername,
                    Name = g.Account.Name,
                    Status = g.Account.Status,
                    ImageUrl = g.Account.ImageUrl
                }));

                return Ok (participants);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Get all groups joined by the authenticated account
        /// </summary>

        // GET messageHandlingApi/Group/
        [HttpGet]
        public IActionResult AccountGroups()
        {
            try
            {
                var acc = Context.Account.Find(User.Identity.Name);
                var accountGroups = acc.AccountGroups.ToList();
                List<Groups> groups = new List<Groups>();

                accountGroups.ForEach(g => groups.Add(new Groups {
                    Id = g.GroupId,
                    Name = g.Group.Name,
                    CreatorUsername = g.Group.CreatorUsername
                }));

                return Ok (groups);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Get group info
        /// </summary>

        // GET messageHandlingApi/Group/{id}
        [HttpGet ("{id}")]
        public IActionResult GroupInfo(int id)
        {
            try
            {
                var group = Context.Groups.Find(id);
                var creator = Context.Account.Find(group.CreatorUsername);
                var accGroup = Context.AccountGroup.Where(g => g.GroupId == id).ToList();
                List<AccountEdit> members = new List<AccountEdit>();

                accGroup.ForEach(
                    ag => members.Add(new AccountEdit
                    {
                        Name = Context.Account.Find(ag.AccountUsername).Name,
                        Status = Context.Account.Find(ag.AccountUsername).Status
                    })
                );

                return Ok (new GroupInfo
                {
                    Name = group.Name,
                    Creator = new AccountEdit
                    {
                        Name = creator.Name,
                        Status = creator.Status,
                    },
                    Participants = accGroup.Count(),
                    ListOfParticipants = members
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Create new group
        /// </summary>

        // POST messageHandlingApi/Group/{name}
        [HttpPost ("{name}")]
        public IActionResult Create(string name, [FromBody] List<Account> participants)
        {
            try
            {
                var account = Context.Account.Find(User.Identity.Name);

                Groups newGroup = new Groups
                {
                    Name = name,
                    CreatorUsername = account.Username
                };

                account.Role = "GroupAdmin";
                newGroup.GroupAccounts = new List<AccountGroup>();

                newGroup.GroupAccounts.Add(new AccountGroup {
                    Group = newGroup,
                    Account = account
                });

                participants.ForEach(p => newGroup.GroupAccounts.Add(new AccountGroup {
                    Group = newGroup,
                    Account = Context.Account.Find(p.Username)
                }));

                Context.Account.Update(account);
                Context.Groups.Add(newGroup);
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Add new member to the group
        /// </summary>

        // POST messageHandlingApi/Group/{groupId}/{username}
        //[Authorize(Roles = "GroupAdmin")]
        [HttpPost]
        public IActionResult AddMember(AccountGroup ag)
        {
            try
            {
                var group = Context.Groups.Find(ag.GroupId);
                var creator = Context.Account.Find(group.CreatorUsername);

                if (ag.AccountUsername == User.Identity.Name)
                    return BadRequest("You can't add yourself to the group");

                if (group == null)
                    return BadRequest(new { message = "Invalid group" });

                if (group.CreatorUsername != User.Identity.Name)
                    return BadRequest(new { message = "You are not the creator of this group" });

                if (Context.Account.Find(ag.AccountUsername) == null)
                    return BadRequest(new { message = "Invalid contact, cannot be added to group" });          
                
                Context.AccountGroup.Add(ag);
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Edit group
        /// </summary>
        // PUT messageHandlingApi/Group/
        //[Authorize(Roles = "GroupAdmin")]
        [HttpPut]
        public IActionResult Edit(Groups grp)
        {
            try
            {
                var group = Context.Groups.Find(grp.Id);

                if (group == null)
                    return BadRequest(new { message = "Invalid group" });

                if (group.CreatorUsername != User.Identity.Name)
                    return BadRequest(new { message = "You are not the creator of this group" });

                group.Name = grp.Name;
                Context.Groups.Update(group);
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Delete a group
        /// </summary>
        // DELETE messageHandlingApi/Group/{id}
        [Authorize(Roles = "GroupAdmin")]
        [HttpDelete ("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var group = Context.Groups.Find(id);

                if (group == null)
                    return NotFound("Group doesn't exist");

                if (group.CreatorUsername != User.Identity.Name)
                    return BadRequest(new { message = "You are not the creator of this group" });

                var accGroup = Context.AccountGroup.Where(g => g.GroupId == id).ToList();
                //var chat = Context.Message.Where(m => m.GroupsId == id).ToList();

                // Delete all Account-Group links
                accGroup.ForEach(
                    ag => Context.AccountGroup.Remove(ag)
                );

                // Delete all group messages
                /*chat.ForEach(
                    c => Context.Message.Remove(c)
                );*/

                Context.SaveChanges();
                Context.Groups.Remove(group);
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