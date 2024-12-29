using ProjectGSMAUI.Api.Controllers.Quy;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Services
{
    public interface IVoucherServices
    {
        Task<List<ActiveVoucher>> GetAll();
        Task<ActiveVoucher> GetByID(int id);
        Task<APIResponse> Create(ActiveVoucher data);
        Task<APIResponse> Remove(int id);
        Task<APIResponse> Update (ActiveVoucher data, int id);
    }
}
