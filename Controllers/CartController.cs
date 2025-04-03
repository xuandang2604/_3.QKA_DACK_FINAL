using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _3.QKA_DACK.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using _3.QKA_DACK.Models.CartModels;
using _3.QKA_DACK.Models.OrderModels;
using _3.QKA_DACK.Models.Another;
using _3.QKA_DACK.Repositories.CartRepo;
using _3.QKA_DACK.Repositories.OrderRepo;

namespace _3.QKA_DACK.Controllers
{
   
   // [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public CartController(ICartRepository cartRepository, IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            //if (userId == null) return RedirectToAction("Login", "Account"); // ✅ bảo vệ

            var cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? new Cart { CartItems = new List<CartItem>() };
            return View(cart);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> CartSummary()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return PartialView("_CartPartial", new Cart { CartItems = new List<CartItem>() });

            var cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? new Cart { CartItems = new List<CartItem>() };
            return PartialView("_CartPartial", cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem != null)
                {
                    cart.CartItems.Remove(cartItem);
                    await _cartRepository.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost("pay")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index");
            }
            var Amount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

            // ✅ Gọi PaymentController để tạo URL VNPay
            return RedirectToAction("CreatePaymentUrlVnpay", "Payment", new
            {
                Name = User.Identity.Name,
                Amount = Amount ,
                OrderDescription = "Thanh toán đơn hàng",
                OrderType = "order"
            });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ConfirmOrder(Order order)
        //{
        //    var userId = _userManager.GetUserId(User);

        //    if (userId == null) return RedirectToAction("Login", "Account");
        //    order.UserId = userId;
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user != null && string.IsNullOrEmpty(user.FullName))
        //    {
        //        user.FullName = "Guest";
        //        await _userManager.UpdateAsync(user);
        //    }

        //    var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        //    if (cart == null || !cart.CartItems.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    order.UserId = userId;
        //    order.InvoiceDate = DateTime.Now;
        //    order.OrderItems = cart.CartItems.Select(ci => new OrderItem
        //    {
        //        ProductId = ci.ProductId,
        //        Quantity = ci.Quantity,
        //        UnitPrice = ci.Product.Price
        //    }).ToList();
        //    order.TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

        //    await _orderRepository.AddOrderAsync(order);
        //    await _orderRepository.SaveChangesAsync();
        //    await ClearCartAsync(cart);

        //    return RedirectToAction("PaymentSuccess", "Order");
        //}

        public async Task ClearCartAsync(Cart cart)
        {
            cart.CartItems.Clear();
            await _cartRepository.SaveChangesAsync(); 
        }
    }
}
