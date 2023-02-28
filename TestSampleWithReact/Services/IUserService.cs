using Microsoft.AspNetCore.Identity;

namespace TestSampleWithReact.Services
{
    public interface IUserService
    {
        Task<byte[]> GetAvatarByIdAsync(string userId);

        Task<IdentityResult> UpdateAvatarAsync(string userId, byte[] avatarBlob);
    }
}