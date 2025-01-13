using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Container
{
    public interface ISanPham
    {
        Task<IEnumerable<SanPham>> GetSanPhamsAsync();
        Task<SanPham> GetSanPhamByIdAsync(int id);
        Task CreateSanPhamAsync(SanPham sanPham);
        Task UpdateSanPhamAsync(SanPham sanPham);
        Task DeleteSanPhamAsync(int id);
    }
}
