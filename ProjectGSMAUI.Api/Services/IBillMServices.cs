using ProjectGSMAUI.Api.Modal;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectGSMAUI.Api.Controllers.Quy;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Services
{
    public interface IBillMServices
    {
        // Phương thức lấy tất cả hóa đơn
        Task<List<Billmodal>> GetAll();

        // Phương thức lấy hóa đơn theo ID
        Task<Billmodal> GetByID(int id);

        // Phương thức tạo hóa đơn mới
        Task<APIResponse> Create(Billmodal data);

        // Phương thức xóa hóa đơn
        Task<APIResponse> Remove(int id);

        // Phương thức cập nhật hóa đơn
        Task<APIResponse> Update(int id);

        Task<Billmodal> GetDetailsByID(int id);
    }
}