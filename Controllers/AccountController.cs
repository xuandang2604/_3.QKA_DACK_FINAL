using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using _3.QKA_DACK.Models.Another;

namespace _3.QKA_DACK.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Profile", "Home")
            }, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Index", "Home");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrWhiteSpace(name))
            {
                name = email.Split('@')[0]; // Lấy phần trước @ của email làm tên
            }
            var picture = User.FindFirstValue("urn:google:picture") ?? "/images/default-avatar.png";

            // Tìm xem user đã có trong DB chưa
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null)
            {
                appUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name
                };

                var result = await _userManager.CreateAsync(appUser);
                if (!result.Succeeded)
                {
                    return Content("Lỗi khi tạo user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var accessToken = authenticateResult?.Properties?.GetTokenValue("access_token");

            var user = new
            {
                Name = name,
                Email = email,
                Picture = picture,
                AccessToken = accessToken ?? "No access token",
            };
            await _userManager.AddToRoleAsync(appUser, "Customer");
            return View((dynamic)user);
        }
    }
}
