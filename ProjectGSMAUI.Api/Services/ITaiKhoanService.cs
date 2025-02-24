using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
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


        //Admin
        Task<List<TaiKhoan>> TaiKhoanAdmin(string Name);
        Task<APIResponse> CreateAdmin(TaiKhoanRequest data);
        Task<APIResponse> GetByTenTaiKhoan(string Name);
        Task<APIResponse> GetByEmail(string Name);
        Task<APIResponse> UpdateAdmin(string id,TaiKhoanRequest data);
        Task<APIResponse> VoHieuHoa(string id);


        //Customer
        Task<List<TaiKhoan>> TaiKhoanCustomer(string Name);
        Task<APIResponse> CreateCustomer(TaiKhoanRequest data);
        Task<APIResponse> UpdateCustomer(string id, TaiKhoanRequest data);
        Task<TaiKhoan> GetTaiKhoanByTenTaiKhoanAsync(string tenTaiKhoan);

        //Customer2
        Task<APIResponse> UpdateCustomer2(string id, TaiKhoanRequest data);


    }
}

