using ProjectGSMVC.Utils;
using ProjectGSMVC.ViewModels;

namespace ProjectGSMVC.Services
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl( HttpContext context, VnPaymentRequestModel model );
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
