using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using _3.QKA_DACK.Models.CartModels;
using _3.QKA_DACK.Models.Another;
using _3.QKA_DACK.Repositories.CartRepo;

namespace _3.QKA_DACK.Areas.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartSummaryViewComponent(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (user == null)
            {
                return View(new Cart { CartItems = new List<CartItem>() }); // Tránh vòng lặp khi user chưa đăng nhập
            }

            var userId = user.Id;  // Lấy userId đúng cách
            Console.WriteLine($"CartSummaryViewComponent được gọi cho userId: {userId}");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            cart ??= new Cart { CartItems = new List<CartItem>() };

            return View("Default", cart);
        }
    }
}
