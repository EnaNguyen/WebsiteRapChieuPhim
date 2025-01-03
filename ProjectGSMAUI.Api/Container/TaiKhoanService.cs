using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectGSMAUI.Api.Utilities;

namespace ProjectGSMAUI.Api.Container
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaiKhoan>> GetTaiKhoansAsync()
        {
            return await _context.TaiKhoans
                .Select(t => new TaiKhoan
                {
                    IdtaiKhoan = t.IdtaiKhoan,
                    TenTaiKhoan = t.TenTaiKhoan,
                    MatKhau = t.MatKhau,
                    TenNguoiDung = t.TenNguoiDung,
                    TrangThai = t.TrangThai,
                    Hinh = t.Hinh,
                    Cccd = t.Cccd
                }).ToListAsync();
        }

        public async Task<TaiKhoan> GetTaiKhoanByIdAsync(string id)
        {
            return await _context.TaiKhoans
                .Where(t => t.IdtaiKhoan == id)
                .Select(t => new TaiKhoan
                {
                    IdtaiKhoan = t.IdtaiKhoan,
                    TenTaiKhoan = t.TenTaiKhoan,
                    MatKhau = t.MatKhau,
                    TenNguoiDung = t.TenNguoiDung,
                    TrangThai = t.TrangThai,
                    Hinh = t.Hinh,
                    Cccd = t.Cccd
                }).FirstOrDefaultAsync();
        }

        public async Task CreateTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            taiKhoan.MatKhau = PasswordHasher.HashPassword(taiKhoan.MatKhau);
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            taiKhoan.MatKhau = PasswordHasher.HashPassword(taiKhoan.MatKhau);
            _context.Entry(taiKhoan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaiKhoanAsync(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
