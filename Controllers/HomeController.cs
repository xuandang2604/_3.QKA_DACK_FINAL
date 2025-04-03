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
using _3.QKA_DACK.Repositories.BrandRepo;
using _3.QKA_DACK.Repositories.ProductRepo;
using _3.QKA_DACK.Models.CategoryModels;
using _3.QKA_DACK.Repositories.CategoryRepo;

namespace _3.QKA_DACK.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LocalizationService _localizer;
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(LocalizationService localizer, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IProductRepository productRepository, IBrandRepository brandRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _localizer = localizer;

        }

        public async Task<IActionResult> Index(int? categoryId, string search)
        {
            var products = await _productRepository.GetAllAsync();
            // Lấy danh sách danh mục từ cơ sở dữ liệu
            var categories = await _categoryRepository.GetAllAsync() ?? new List<Category>();

            // Tìm tất cả danh mục cha (ParentCategory == NULL)
            ViewBag.ParentCategories = categories.Where(c => c.ParentCategory == null).ToList();

            // Lấy danh mục con nếu có categoryId
            if (categoryId.HasValue)
            {
                ViewBag.ChildCategories = categories.Where(c => c.ParentCategoryId == categoryId.Value).ToList();
            }
            else
            {
                ViewBag.ChildCategories = new List<Category>();
            }

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (categoryId.HasValue)
            {
                // Tìm tất cả danh mục con của danh mục cha
                var selectedCategoryIds = GetAllChildCategoryIds(categoryId.Value, categories.ToList());

                // Lọc sản phẩm theo danh mục cha và con
                products = products.Where(p => selectedCategoryIds.Contains((int)p.CategoryId)).ToList();
            }

            // Lấy thông tin thương hiệu cho từng sản phẩm
            foreach (var product in products)
            {
                if (product.BrandId.HasValue)
                {
                    product.Brand = await _brandRepository.GetById(product.BrandId.Value);
                }
            }

            ViewBag.Search = search;
            return View(products);
        }
        private List<int> GetAllChildCategoryIds(int parentId, List<Category> categories)
        {
            // Lấy danh mục con của danh mục cha
            var childCategories = categories.Where(c => c.ParentCategoryId == parentId).ToList();

            // Khởi tạo danh sách ID danh mục (bao gồm cha và con)
            var categoryIds = new List<int> { parentId }; // Bắt đầu với danh mục cha

            foreach (var category in childCategories)
            {
                categoryIds.AddRange(GetAllChildCategoryIds(category.Id, categories));
            }

            return categoryIds; // Trả về danh sách ID của cha và con
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
