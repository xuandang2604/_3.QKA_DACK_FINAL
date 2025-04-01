using Microsoft.AspNetCore.Mvc;
using _3.QKA_DACK.Repositories.VnPayRepo;
namespace _3.QKA_DACK.Controllers
{
    public class Checkout : Controller
    {
        private readonly IVnPayService _vnPayService;
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }
    }
}
