using AutoMapper;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;

namespace ProjectGSMAUI.Api.Container
{
    public class CheckOutServices : ICheckOutServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<TaiKhoanServices> logger;
        public CheckOutServices(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<APIResponse> CreateDatVe(List<DatVeCreator> data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                int? MaxHoaDonNullable = _context.HoaDons
                .OrderByDescending(g => g.MaHoaDon)
                .Select(g => g.MaHoaDon)
                .FirstOrDefault();
                int MaxHoaDon = MaxHoaDonNullable +1 ?? 0;
                int TongTienHoaDon = 0;
                HoaDon newHD = new HoaDon()
                {
                    MaHoaDon = MaxHoaDon,
                    MaKhachHang = data[0].MaKhachHang,
                    TongTien = 0,
                    MaGiamGia = null,
                    NgayXuat = DateOnly.FromDateTime(DateTime.Now),
                    TinhTrang = 1
                };
                _context.HoaDons.Add(newHD);
                await _context.SaveChangesAsync();
                foreach (var item in data)
                {
                    ChiTietHoaDon newCT = new ChiTietHoaDon()
                    {
                        MaVe= item.MaVe,
                        MaHoaDon = MaxHoaDon,
                        Gia = 100000
                    };
                    TongTienHoaDon += 100000;
                    _context.ChiTietHoaDons.Add(newCT);
                }
                await _context.SaveChangesAsync();
                newHD.TongTien = TongTienHoaDon;
                _context.HoaDons.Update(newHD);
                 await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.ErrorMessage = ex.Message;
                response.ResponseCode = 400;
            }
            return response;
        }
    }
}
