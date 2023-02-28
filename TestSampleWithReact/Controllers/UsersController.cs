using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestSampleWithReact.Models;
using TestSampleWithReact.Services;

namespace TestSampleWithReact.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public UsersController(UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger,
            IUserService userService)
        {
            _userManager = userManager;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var isUserAdmin = User.IsInRole("Admin");

            var listOfUsers = await _userManager.Users.Select(u => new
            {
                UserId = u.Id,
                Name = u.UserName,
                Avatar = Convert.ToBase64String(u.Avatar),
                Actions = isUserAdmin ? new[] { "Delete" } : new string[0]
            }).ToListAsync();

            return Ok(new { isAdmin = isUserAdmin, users = listOfUsers });
        }


        [HttpGet("avatar/{userId}")]
        public async Task<IActionResult> GetUserAvatar(string userId)
        {
            return Ok(Convert.ToBase64String(await _userService.GetAvatarByIdAsync(userId)));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.DeleteAsync(user);

            return Ok();
        }
    }
}
