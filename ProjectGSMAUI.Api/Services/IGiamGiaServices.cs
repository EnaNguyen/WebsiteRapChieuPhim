using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Services
{
    public interface IGiamGiaServices
    {
        Task<List<ActiveGiamGia>> GetAll(string name);
        Task<ActiveGiamGia> GetByID(int id);
		Task<APIResponse> Create(GiamGiaRequest data);
		Task<APIResponse> Remove(int id);
		Task<APIResponse> Update(GiamGiaRequest data, int id);

    }
}
