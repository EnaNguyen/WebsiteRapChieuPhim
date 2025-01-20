using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Services
{
    public interface IHinhAnhServices
    {
        Task<List<HinhAnh>> GetAll(string name);
        Task<HinhAnh> GetByID(int id);
        Task<APIResponse> Create(HinhAnh data);
        Task<APIResponse> Remove(int id);
        Task<APIResponse> Update(HinhAnh data, int id);
    }
}
