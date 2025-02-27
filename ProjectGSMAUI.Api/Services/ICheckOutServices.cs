using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Services
{
    public interface ICheckOutServices
    {
        Task<int> CreateDatVe(HoaDonCreator data);
        Task<APIResponse> OrderSanPham(List<SanPhamBuying> data);
        Task<List<ComboView>> ListCombo();
        Task<APIResponse> OrderCombo(List<ComboBuying> data);

    }
}
