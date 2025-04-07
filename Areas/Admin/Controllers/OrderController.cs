using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _3.QKA_DACK.Models.OrderModels;
using _3.QKA_DACK.Models.Another;
using _3.QKA_DACK.Repositories.CartRepo;
using _3.QKA_DACK.Repositories.OrderRepo;
using _3.QKA_DACK.Repositories.VnPayRepo;
using _3.QKA_DACK.Models.CartModels;


namespace _3.QKA_DACK.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class OrderController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVnPayService _vnPayRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderRepository orderRepository, IVnPayService vnPayRepository, UserManager<ApplicationUser> userManager,ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _vnPayRepository = vnPayRepository;
            _userManager = userManager;
            _cartRepository = cartRepository;
        }

        public async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            order.User = await _userManager.FindByIdAsync(order.UserId);
            if (order == null)
            {
                return NotFound();
            }

            // Lấy UserId hiện tại
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Check role admin
            bool isAdmin = User.IsInRole("Admin");
            bool isEmployee = User.IsInRole("Employee");
            // Nếu không phải admin và không phải order của mình => cấm truy cập
            if (!isAdmin && order.UserId != currentUserId && !isEmployee)
            {
                return Forbid(); // 403
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                _orderRepository.RemoveOrder(order);
                await _orderRepository.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Cart");
        }

        //[HttpGet]
        //public IActionResult PaymentCallbackVnpay()
        //{
        //    var response = _vnPayRepository.PaymentExecute(Request.Query);
        //    //_vnPayRepository.SavePaymentResponseAsync(response);

        //    return View(response);
        //}
      
        public async Task<IActionResult> Index()
        {
            bool isAdmin = User.IsInRole("Admin");
            bool isEmployee = User.IsInRole("Employee");
            IEnumerable<Order> orders;

            if (isAdmin || isEmployee)
            {
                orders = await _orderRepository.GetAllOrdersAsync();
                foreach (var order in orders)
                {
                    order.User = await _userManager.FindByIdAsync(order.UserId);
                }
            }
            else
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                orders = await _orderRepository.GetOrdersByUserIdAsync(currentUserId);
            }

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayRepository.PaymentExecute(Request.Query);
            
            if (response.Success) // ✅ Nếu thanh toán thành công
            {
                var userId = _userManager.GetUserId(User);
                // ✅ Kiểm tra và chuyển đổi OrderId từ string sang int


                    if (response.VnPayResponseCode == "00")
                    {
                        // ✅ Lưu thông tin thanh toán
                        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                        var order = new Order
                        {
                            UserId = userId,
                            InvoiceDate = DateTime.Now,
                            OrderItems = cart.CartItems.Select(ci => new OrderItem
                            {
                                ProductId = ci.ProductId,
                                Quantity = ci.Quantity,
                                UnitPrice = ci.Product.Price
                            }).ToList(),
                            TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                            Status = "Paid"
                        };

                        // ✅ Lưu order trước khi chuyển sang VNPay
                        await _orderRepository.AddOrderAsync(order);
                        await _orderRepository.SaveChangesAsync();
                    }

                // ✅ Xóa giỏ hàng của người dùng
                
                if (!string.IsNullOrEmpty(userId))
                {
                    var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                    if (cart != null && cart.CartItems.Any())
                    {
                        cart.CartItems.Clear();
                        await _cartRepository.SaveChangesAsync();
                    }
                }

                return RedirectToAction("PaymentSuccess");
            }

            return RedirectToAction("PaymentFailed"); // ✅ Nếu thất bại, điều hướng sang trang thất bại
        }

        [HttpGet]
        public IActionResult PaymentFailed()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            return View();
        }



        //Hien thi doanh thu theo thang
        public async Task<IActionResult> MonthlyRevenue()
        {
            if (!User.IsInRole("Employee"))
            {
                return Forbid();
            }

            var revenueData = await _orderRepository.GetMonthlyRevenueAsync();
            return View(revenueData);
        }



    }


}
    