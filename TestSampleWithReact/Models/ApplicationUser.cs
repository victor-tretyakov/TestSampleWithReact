using Microsoft.AspNetCore.Identity;

namespace TestSampleWithReact.Models
{
    public class ApplicationUser : IdentityUser
    {
        public byte[] Avatar { get; set; } = new byte[0];
    }
}