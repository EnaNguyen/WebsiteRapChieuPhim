using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Services
{
    public interface ITheLoaiPhimService
    {
        Task<APIResponse> GetAll();
        Task<TheLoaiPhim> GetByID(string id);
        Task<APIResponse> Create(TheLoaiPhim data);
        Task<APIResponse> Remove(string id);
        Task<APIResponse> Update(TheLoaiPhim data, string id);
    }
}