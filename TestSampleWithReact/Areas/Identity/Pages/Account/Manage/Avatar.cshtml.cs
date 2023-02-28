// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestSampleWithReact.Models;
using TestSampleWithReact.Services;

namespace TestSampleWithReact.Areas.Identity.Pages.Account.Manage
{
    public class AvatarModel : PageModel
    {
        private readonly ReadOnlyCollection<string> _supportedExtentions = new ReadOnlyCollection<string>(
            new List<string>
            {
                "png", "jpg", "bmp"
            });

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;

        public AvatarModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public string AvatarSrc { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Avatar")]
            public IFormFile Avatar { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var avatar = await _userService.GetAvatarByIdAsync(user.Id);

            AvatarSrc = Convert.ToBase64String(avatar);

            Input = new InputModel();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile postedFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            string fileName = Path.GetFileName(postedFile.FileName);

            var fileExtention = fileName.Substring(fileName.LastIndexOf('.') + 1);

            if (_supportedExtentions.Any(ext => ext == fileExtention) == false)
            {
                StatusMessage = "Unsupported file extentions. Please choose image file with supported extentions.";
                return RedirectToPage();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                postedFile.CopyTo(ms);

                var result = await _userService.UpdateAvatarAsync(user.Id, ms.ToArray());

                if (!result.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            StatusMessage = "Your avatar has been uploaded";

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
