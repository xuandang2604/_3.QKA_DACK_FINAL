using _3.QKA_DACK.Models.VNPay;

namespace _3.QKA_DACK.Repositories.VnPayRepo
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
