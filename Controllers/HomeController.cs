using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using _3.QKA_DACK.Models;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using _3.QKA_DACK.Models.Another;
using _3.QKA_DACK.Resources;

namespace _3.QKA_DACK.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LocalizationService _localizer;

        public HomeController(LocalizationService localizer, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;

        }

        public IActionResult Index()
        {
            //return RedirectToAction("Index", "Product", new { area = "Admin" });
          //  var Welcome= _localizer.GetLocalizedHtmlString("Welcome");
            return View();
        }   

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Home");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
            var picture = authenticateResult.Principal.FindFirstValue("urn:google:picture") ?? "/images/default-avatar.png";

            if (email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Tạo user mới
                user = new ApplicationUser
                {
                    UserName = email,
                    FullName = string.IsNullOrEmpty(name) ? email : name,
                    Email = email,
                    NormalizedUserName = email.ToUpper(),
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Gán role Customer cho user mới
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            else
            {
                // Nếu user đã có, nhưng chưa có role cũng gán luôn
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Customer") && !roles.Contains("Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}
