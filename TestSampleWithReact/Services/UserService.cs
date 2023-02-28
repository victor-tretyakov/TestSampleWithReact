using Microsoft.AspNetCore.Identity;
using TestSampleWithReact.Models;

namespace TestSampleWithReact.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ILogger<UserService> logger,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<byte[]> GetAvatarByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user?.Avatar ?? new byte[0];
        }

        public async Task<IdentityResult> UpdateAvatarAsync(string userId, byte[] avatarBlob)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "404", Description = "User has not been found." }));
            }

            user.Avatar = avatarBlob;

            await _userManager.UpdateAsync(user);

            return await Task.FromResult(IdentityResult.Success);
        }
    }
}
