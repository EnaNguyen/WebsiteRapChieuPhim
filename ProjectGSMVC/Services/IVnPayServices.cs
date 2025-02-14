using ProjectGSMVC.Utils;
using ProjectGSMVC.Models;

namespace ProjectGSMVC.Services
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl( HttpContext context, VnPaymentRequestModel model );
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
