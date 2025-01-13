using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectGSMAUI.Api.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using AutoMapper;

namespace ProjectGSMAUI.Api.Container
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<TaiKhoanService> logger;

        public TaiKhoanService(ApplicationDbContext context, IMapper mapper, ILogger<TaiKhoanService> _logger)
        {
            _context = context;
            this.mapper = mapper;
            logger = _logger;
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
                    Email = t.Email,
                    Sdt = t.Sdt,
                    VaiTro = t.VaiTro,
                    NgaySinh =t.NgaySinh,
                    NgayDangKy = t.NgayDangKy,
                    TrangThai = t.TrangThai,
                    DiemTichLuy = t.DiemTichLuy,
                    Hinh = t.Hinh,
                    Cccd = t.Cccd,
                    GioiTinh = t.GioiTinh,
                    DiaChi = t.DiaChi,
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

        //Admin Services
        public async Task<List<TaiKhoan>> TaiKhoanAdmin(string Name)
        {
            List<TaiKhoan> _response = new List<TaiKhoan>();
            if (Name == null)
            {
                var _data = await this._context.TaiKhoans.Where(g => g.VaiTro == 2)
                .ToListAsync();
                if (_data != null)
                {
                    _response = _data;
                }
            }
            else
            {
                var _data = await this._context.TaiKhoans.Where(g => g.VaiTro == 2 && ((g.TenTaiKhoan.Contains(Name)) || (g.TenNguoiDung.Contains(Name)) || (g.Sdt.Contains(Name)) || (g.Email.Contains(Name))))
               .ToListAsync();
                if (_data != null)
                {
                    _response = _data;
                }
                if (_data != null)
                {
                    {
                        _response = _data;
                    }
                }
            }
            return _response;
        }
        public async Task<APIResponse> CreateAdmin(TaiKhoanRequest data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
               
                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(data.Hinh))
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(data.Hinh);
                    }
                    catch (FormatException)
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                        return response;
                    }
                }
                int maxMaAdmin = _context.TaiKhoans.Where(t => t.VaiTro==2)
                    .AsNoTracking()
                    .AsEnumerable() 
                    .OrderByDescending(g => int.Parse(g.IdtaiKhoan.Substring(2))) 
                    .Select(g => int.Parse(g.IdtaiKhoan.Substring(2))) 
                    .FirstOrDefault();

                maxMaAdmin += 1; 


                var Tk = new TaiKhoan
                {
                    IdtaiKhoan = "AD"+maxMaAdmin.ToString("000"),
                    TenNguoiDung = data.TenNguoiDung,
                    MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                    TenTaiKhoan =data.TenTaiKhoan,
                    Email= data.Email,
                    Sdt= data.Sdt,
                    VaiTro= 2,
                    NgaySinh= data.NgaySinh,
                    NgayDangKy= DateOnly.FromDateTime(DateTime.Today),
                    TrangThai = 1,
                    DiemTichLuy =0,
                    Hinh =imageBytes,
                    Cccd= data.Cccd,
                    GioiTinh=data.GioiTinh,
                    DiaChi =data.DiaChi,
                };

                await this._context.TaiKhoans.AddAsync(Tk);
                await this._context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.ResponseCode = 201;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
        public async Task<APIResponse> GetByTenTaiKhoan(string Name)
        {
            APIResponse response = new APIResponse();
            var _data = await _context.TaiKhoans
                .FirstOrDefaultAsync(g => g.TenTaiKhoan == Name);

            if (_data != null)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Tên Tài Khoản đã tồn tại";
                return response;
            }
            response.ResponseCode = 201;
            return response;
        }
        public async Task<APIResponse> GetByEmail(string Name)
        {
            APIResponse response = new APIResponse();
            var _data = await _context.TaiKhoans
                .FirstOrDefaultAsync(g => g.Email == Name);

            if (_data != null)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Email này đã tồn tại";
                return response;
            }
            response.ResponseCode = 201;
            return response;
        }
        public async Task<APIResponse> UpdateAdmin(string id,TaiKhoanRequest data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(data.Hinh))
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(data.Hinh);
                    }
                    catch (FormatException)
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                        return response;
                    }
                }
                var Tk = await _context.TaiKhoans.Where(g => g.IdtaiKhoan == id).FirstOrDefaultAsync();




                Tk.TenNguoiDung = data.TenNguoiDung;
                Tk.MatKhau = PasswordHasher.HashPassword(data.MatKhau);
                Tk.TenTaiKhoan = data.TenTaiKhoan;
                Tk.Email = data.Email;
                Tk.Sdt = data.Sdt;
                Tk.VaiTro = 2;
                Tk.NgaySinh = data.NgaySinh;
                Tk.NgayDangKy = DateOnly.FromDateTime(DateTime.Today);
                Tk.TrangThai = 1;
                Tk.DiemTichLuy = 0;

                Tk.Cccd = data.Cccd;
                Tk.GioiTinh = data.GioiTinh;
                Tk.DiaChi = data.DiaChi;
                if(data.Hinh!=null)
                {
                    Tk.Hinh = imageBytes;
                }    

                this._context.TaiKhoans.Update(Tk);
                await this._context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.ResponseCode = 201;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
        public async Task<APIResponse> VoHieuHoa(string id)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var Tk = await _context.TaiKhoans.Where(g => g.IdtaiKhoan == id).FirstOrDefaultAsync();
                if (Tk != null)
                {
                    if (Tk.TrangThai == 1)
                    {
                        Tk.TrangThai = 0;
                        response.Result = "0";
                    }                                           
                    else
                    {
                        Tk.TrangThai = 1;
                        response.Result = "1";
                    }    
                    this._context.TaiKhoans.Update(Tk);
                    await this._context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.ResponseCode = 201;
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        //Customer Services
        public async Task<List<TaiKhoan>> TaiKhoanCustomer(string Name)
        {
            List<TaiKhoan> _response = new List<TaiKhoan>();
            if (Name == null)
            {
                var _data = await this._context.TaiKhoans.Where(g => g.VaiTro == 1)
                .ToListAsync();
                if (_data != null)
                {
                    _response = _data;
                }
            }
            else
            {
                var _data = await this._context.TaiKhoans.Where(g => g.VaiTro == 1 && ((g.TenTaiKhoan.Contains(Name)) || (g.TenNguoiDung.Contains(Name)) || (g.Sdt.Contains(Name)) || (g.Email.Contains(Name))))
               .ToListAsync();
                if (_data != null)
                {
                    _response = _data;
                }
                if (_data != null)
                {
                    {
                        _response = _data;
                    }
                }
            }
            return _response;
        }
        public async Task<APIResponse> CreateCustomer(TaiKhoanRequest data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(data.Hinh))
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(data.Hinh);
                    }
                    catch (FormatException)
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                        return response;
                    }
                }
                int maxMaAdmin = _context.TaiKhoans.Where(t=>t.VaiTro==1)
                    .AsNoTracking()
                    .AsEnumerable()
                    .OrderByDescending(g => int.Parse(g.IdtaiKhoan.Substring(2)))
                    .Select(g => int.Parse(g.IdtaiKhoan.Substring(2)))
                    .FirstOrDefault();

                maxMaAdmin += 1;


                var Tk = new TaiKhoan
                {
                    IdtaiKhoan = "TK" + maxMaAdmin.ToString("000"),
                    TenNguoiDung = data.TenNguoiDung,
                    MatKhau = PasswordHasher.HashPassword(data.MatKhau),
                    TenTaiKhoan = data.TenTaiKhoan,
                    Email = data.Email,
                    Sdt = data.Sdt,
                    VaiTro = 1,
                    NgaySinh = data.NgaySinh,
                    NgayDangKy = DateOnly.FromDateTime(DateTime.Today),
                    TrangThai = 1,
                    DiemTichLuy = 0,
                    Hinh = imageBytes,
                    Cccd = data.Cccd,
                    GioiTinh = data.GioiTinh,
                    DiaChi = data.DiaChi,
                };

                await this._context.TaiKhoans.AddAsync(Tk);
                await this._context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.ResponseCode = 201;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
        public async Task<APIResponse> UpdateCustomer(string id, TaiKhoanRequest data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(data.Hinh))
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(data.Hinh);
                    }
                    catch (FormatException)
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                        return response;
                    }
                }
                var Tk = await _context.TaiKhoans.Where(g => g.IdtaiKhoan == id).FirstOrDefaultAsync();




                Tk.TenNguoiDung = data.TenNguoiDung;
                Tk.MatKhau = PasswordHasher.HashPassword(data.MatKhau);
                Tk.TenTaiKhoan = data.TenTaiKhoan;
                Tk.Email = data.Email;
                Tk.Sdt = data.Sdt;
                Tk.VaiTro = 1;
                Tk.NgaySinh = data.NgaySinh;
                Tk.NgayDangKy = DateOnly.FromDateTime(DateTime.Today);
                Tk.TrangThai = 1;
                Tk.DiemTichLuy = 0;

                Tk.Cccd = data.Cccd;
                Tk.GioiTinh = data.GioiTinh;
                Tk.DiaChi = data.DiaChi;
                if (data.Hinh != null)
                {
                    Tk.Hinh = imageBytes;
                }

                this._context.TaiKhoans.Update(Tk);
                await this._context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.ResponseCode = 201;
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
