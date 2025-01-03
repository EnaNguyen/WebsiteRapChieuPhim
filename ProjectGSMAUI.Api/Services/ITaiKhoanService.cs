using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Container
{
    public interface ITaiKhoanService
    {
        Task<IEnumerable<TaiKhoan>> GetTaiKhoansAsync();
        Task<TaiKhoan> GetTaiKhoanByIdAsync(string id);
        Task CreateTaiKhoanAsync(TaiKhoan taiKhoan);
        Task UpdateTaiKhoanAsync(TaiKhoan taiKhoan);
        Task DeleteTaiKhoanAsync(string id);
    }
}
