using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public interface ILichChieuService
    {
        Task<APIResponse> GetAll();
        Task<LichChieu> GetByID(int id);
        Task<APIResponse> GenerateSchedule();
        Task<APIResponse> AddSchedule(LichChieu schedule);
        Task<APIResponse> DeleteSchedule(int id);
        Task<APIResponse> GenerateScheduleWithAdjustments(int phimCanTangId, int soSuatCanTang); // Phương thức mới
        Task<List<LichChieuView>> GetLichChieuByDate(int id, DateOnly date);

    }
}