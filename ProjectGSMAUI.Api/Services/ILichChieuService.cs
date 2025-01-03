using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Services
{
    public interface ILichChieuService
    {
        Task<APIResponse> GetAll();
        Task<LichChieu> GetByID(int id);
        Task<APIResponse> GenerateSchedule();
    }
}