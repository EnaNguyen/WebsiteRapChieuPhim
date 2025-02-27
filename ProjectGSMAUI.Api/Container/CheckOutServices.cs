using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using System.Transactions;

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
        public async Task<int> CreateDatVe(HoaDonCreator data)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            int? MaxHoaDonNullable = _context.HoaDons
                .OrderByDescending(g => g.MaHoaDon)
                .Select(g => g.MaHoaDon)
                .FirstOrDefault();
            int MaxHoaDon = MaxHoaDonNullable + 1 ?? 0;
            try 
            {
                
                int TongTienHoaDon = 0;
                HoaDon newHD = new HoaDon()
                {
                    MaHoaDon = MaxHoaDon,
                    MaKhachHang = data.ListCT[0].MaKhachHang,
                    TongTien = data.TongTien,
                    NgayXuat = DateOnly.FromDateTime(DateTime.Now),
                    TinhTrang = 1,
                    MaGiamGia = data.MaGiamGia
                };
                _context.HoaDons.Add(newHD);
                await _context.SaveChangesAsync();
                foreach (var item in data.ListCT)
                {
                    ChiTietHoaDon newCT = new ChiTietHoaDon()
                    {
                        MaVe= item.MaVe,
                        MaHoaDon = MaxHoaDon,
                        Gia = 100000
                    };
                    _context.ChiTietHoaDons.Add(newCT);
                }
                 await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
            return MaxHoaDon;
        }

        public async Task<List<ComboView>> ListCombo()
        {
            var Combo = await _context.Combos.ToListAsync();
            List<ComboView> Result = new List<ComboView>();
            if (Combo.Count != 0)
            {
                
                foreach (var item in Combo)
                {
                    var Temp = await _context.ChiTietCombos
                                             .Where(g => g.ComboId == item.Id)
                                             .ToListAsync();

                    ComboView cbn = new ComboView();
                    List<SanPhamInCombo> splist = new List<SanPhamInCombo>();

                    for (int i = 0; i < Temp.Count; i++)
                    {
                        var sp = await _context.SanPhams
                                               .Where(g => g.Id == Temp[i].SanPhamId)
                                               .Select(g => new { g.TenSanPham })
                                               .FirstOrDefaultAsync();

                        if (sp != null)
                        {
                            SanPhamInCombo s = new SanPhamInCombo();
                            s.Name = sp.TenSanPham;
                            s.SoLuong = Temp[i].SoLuong;
                            splist.Add(s);
                        }
                    }
                    cbn.Id = item.Id;
                    cbn.Hinh = item.HinhAnh;
                    cbn.Name = item.TenCombo;
                    cbn.sanPhams = splist;
                    cbn.Gia = (int)item.Gia;
                    Result.Add(cbn);
                }

            }
            return Result;
        }

        public async Task<APIResponse> OrderSanPham(List<SanPhamBuying> data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? maxId = _context.ChiTietHoaDon1s
                    .OrderByDescending(g => g.MaChiTietHoaDon)
                    .Select(g => (int?)g.MaChiTietHoaDon)
                    .FirstOrDefault();

                int MaxMaChiTiet1;
                if (maxId.HasValue)
                {
                    MaxMaChiTiet1 = maxId.Value + 1;
                }
                else
                {
                    MaxMaChiTiet1 = 1; 
                }

                foreach (var item in data)
                {
                    var DonGia = (int)_context.SanPhams.Where(g => g.Id == item.Id).Select(g => g.Gia).FirstOrDefault();
                    ChiTietHoaDon1 newCT = new ChiTietHoaDon1()
                    {
                        MaHoaDon = item.MaHoaDon,
                        MaSanPham = item.Id,
                        SoLuong = item.quantity,
                        Gia = item.quantity * DonGia
                    };
                    _context.ChiTietHoaDon1s.Add(newCT);
                    MaxMaChiTiet1++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); 
                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Lỗi rồi: " + ex.Message;
                response.ResponseCode = 400;
                await transaction.RollbackAsync();
            }

            return response;
        }
        public async Task<APIResponse> OrderCombo(List<ComboBuying> data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? maxId = _context.ChiTietHoaDon2s
                    .OrderByDescending(g => g.MaChiTietHoaDon)
                    .Select(g => (int?)g.MaChiTietHoaDon)
                    .FirstOrDefault();

                int MaxMaChiTiet1;
                if (maxId.HasValue)
                {
                    MaxMaChiTiet1 = maxId.Value + 1;
                }
                else
                {
                    MaxMaChiTiet1 = 1;
                }

                foreach (var item in data)
                {
                    var DonGia = (int)_context.Combos.Where(g => g.Id == item.Id).Select(g => g.Gia).FirstOrDefault();
                    ChiTietHoaDon2 newCT = new ChiTietHoaDon2()
                    {
                        MaHoaDon = item.MaHoaDon,
                        MaCombo = item.Id,
                        SoLuong = item.quantity,
                        Gia = item.quantity * DonGia
                    };
                    _context.ChiTietHoaDon2s.Add(newCT);
                    MaxMaChiTiet1++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                response.ResponseCode = 200;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Lỗi rồi: " + ex.Message;
                response.ResponseCode = 400;
                await transaction.RollbackAsync();
            }

            return response;
        }
    }
}
