using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Container
{
    public class SanPhamService : ISanPham
    {
        private readonly ApplicationDbContext _context;

        public SanPhamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SanPham>> GetSanPhamsAsync()
        {
            return await _context.SanPhams.ToListAsync();
        }

        public async Task<SanPham> GetSanPhamByIdAsync(int id)
        {
            return await _context.SanPhams.FindAsync(id);
        }

        public async Task CreateSanPhamAsync(SanPham sanPham)
        {
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSanPhamAsync(SanPham sanPham)
        {
            var existingSanPham = await _context.SanPhams.FindAsync(sanPham.Id);
            if (existingSanPham == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại");
            }

            existingSanPham.TenSanPham = sanPham.TenSanPham;
            existingSanPham.Gia = sanPham.Gia;
            existingSanPham.MoTa = sanPham.MoTa;
            _context.SanPhams.Update(existingSanPham);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSanPhamAsync(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }
        }
    }
}
