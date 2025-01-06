using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
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
                    Cccd = t.Cccd,
                    VaiTro = t.VaiTro,
                    Email = t.Email,
                    Sdt = t.Sdt,
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
                    Cccd = t.Cccd,
                    VaiTro = t.VaiTro,
                    Email = t.Email,
                    Sdt = t.Sdt,
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
            var existingAccount = await _context.TaiKhoans.FindAsync(taiKhoan.IdtaiKhoan);
            if (existingAccount == null)
            {
                throw new Exception("Tài khoản không tồn tại.");
            }

            existingAccount.TenTaiKhoan = taiKhoan.TenTaiKhoan;
            existingAccount.MatKhau = taiKhoan.MatKhau;
            existingAccount.Email = taiKhoan.Email;
            existingAccount.TenNguoiDung = taiKhoan.TenNguoiDung;
            existingAccount.Sdt = taiKhoan.Sdt;
            existingAccount.DiaChi = taiKhoan.DiaChi;
            existingAccount.NgaySinh = taiKhoan.NgaySinh;

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
