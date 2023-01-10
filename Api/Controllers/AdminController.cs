using Api.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUsers> _userManager;

        public AdminController(UserManager<AppUsers> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("user-with-roles")]
        public async Task<ActionResult> GetUserWithRole()
        {
            var user = await _userManager.Users
                .Include(r => r.UserRole)
                    .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    Role = u.UserRole.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();
            return Ok(user);
        }
        [Authorize(Policy = "Admin")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();
            var user = await _userManager.FindByNameAsync(username);
            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
                return BadRequest("failed to add");
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("failed to Remove ");
            return Ok(await _userManager.GetRolesAsync(user));
        }
        //[Authorize(Policy = "ModeratePhoto")]
        //[HttpGet("photo-to-moderate")]
        //public ActionResult GetPhotoToModerate()
        //{
        //    return Ok("Moderate and admin");
        //}
    }
}
