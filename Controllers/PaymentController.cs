using Microsoft.AspNetCore.Mvc;
using _3.QKA_DACK.Models.VNPay;
using _3.QKA_DACK.Repositories.VnPayRepo;

namespace _3.QKA_DACK.Controllers
{
   
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
    }
}
