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
        /// Get a list of all groups
        /// </summary>

        // GET messageHandlingApi/Group/list
        [HttpGet ("list")]
        public IActionResult GroupList()
        {
            return Ok (Context.Groups);
        }

        /// <summary>
        /// Get all groups created by the authenticated account
        /// </summary>

        // GET messageHandlingApi/Group/
        [HttpGet]
        public IActionResult AccountGroups()
        {
            var accountGroups = Context.Groups.Where(g => g.CreatorUsername == User.Identity.Name);

            return Ok (accountGroups);
        }

        /// <summary>
        /// Get group info
        /// </summary>

        // GET messageHandlingApi/Group/{id}
        [HttpGet ("{id}")]
        public IActionResult GroupInfo(int id)
        {
            var group = Context.Groups.Find(id);
            var creator = Context.Account.Find(group.CreatorUsername);
            var accGroup = Context.AccountGroup.Where(g => g.GroupId == id).ToList();
            List<AccountEdit> members = new List<AccountEdit>();

            accGroup.ForEach(
                ag => members.Add(new AccountEdit
                {
                    Name = Context.Account.Find(ag.Username).Name,
                    Status = Context.Account.Find(ag.Username).Status
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

        /// <summary>
        /// Create new group
        /// </summary>

        // POST messageHandlingApi/Group/{name}
        [HttpPost ("{name}")]
        public void Create(string name)
        {
            var account = Context.Account.Find();

            Groups newGroup = new Groups
            {
                Name = name,
                CreatorUsername = account.Username
            };

            account.Role = "GroupAdmin";

            Context.Account.Update(account);
            Context.Groups.Add(newGroup);
            Context.SaveChanges();
        }

        /// <summary>
        /// Add new member to the group
        /// </summary>

        // POST messageHandlingApi/Group/{groupId}/{username}
        //[Authorize(Roles = "GroupAdmin")]
        [HttpPost ("{groupId}/{contact}")]
        public IActionResult AddMember(int groupId, string contact)
        {
            var group = Context.Groups.Find(groupId);
            var creator = Context.Account.Find(group.CreatorUsername);

            if (group == null)
                return BadRequest(new { message = "Invalid group" });

            if (group.CreatorUsername != User.Identity.Name)
                return BadRequest(new { message = "You are not the creator of this group" });

            if (Context.Account.Find(contact) == null)
                return BadRequest(new { message = "Invalid contact, cannot be added to group" });
            
            AccountGroup ag = new AccountGroup
            {
                Username = contact,
                GroupId = groupId
            };
            
            Context.AccountGroup.Add(ag);
            Context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Edit group
        /// </summary>

        // POST messageHandlingApi/Group/{name}
        [HttpPut ("{id}/name")]
        public IActionResult Edit(int id, string name)
        {
            var group = Context.Groups.Find(id);

            if (group == null)
                return BadRequest(new { message = "Invalid group" });

            if (group.CreatorUsername != User.Identity.Name)
                return BadRequest(new { message = "You are not the creator of this group" });

            group.Name = name;
            Context.Groups.Update(group);
            Context.SaveChanges();

            return Ok();
        }
    }
}