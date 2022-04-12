using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController :BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRoles")]
        [HttpGet("user-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {

            var users = await _userManager.Users
            .Include(r=>r.UserRoles)
            .ThenInclude(r=>r.Role)
            .OrderBy(u=>u.UserName)
            .Select(u=> new {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r=>r.Role.Name)
            })
            .ToListAsync();
            return Ok(users);
            //return Ok("Only Admin can see this");
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles){

            var user = await _userManager.FindByNameAsync(username);
            if(user == null) return NotFound("Not found");

            var selectedroles = roles.Split(",").Select(x=>x.ToLower()).ToArray();

            var userroles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedroles.Except(userroles.Select(x=>x.ToLower())) );
            if(!result.Succeeded) return BadRequest("Error");

            result = await _userManager.RemoveFromRolesAsync(user, userroles.Select(x=>x.ToLower()).Except(selectedroles));

            if(!result.Succeeded) return BadRequest("Error");

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photo-to-moderate")]
        public ActionResult GetPhotosWithModeration(){
            return Ok("Only Admin and  Moderator can see this");
        }
    }
}