using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.WebSockets;

namespace ProjectGSMAUI.Api.Container
{
    public class BillMServices : IBillMServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BillMServices> _logger;

        public BillMServices(ApplicationDbContext context, IMapper mapper, ILogger<BillMServices> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<Billmodal>> GetAll()
        {
            List<Billmodal> _response = new List<Billmodal>();

            // Lấy dữ liệu hóa đơn và bao gồm cả bảng ChiTietHoaDon + Ve
            var _data = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.MaVeNavigation)  // Bao gồm thông tin từ bảng Ve
                .ToListAsync();

            if (_data != null)
            {
                _response = _mapper.Map<List<HoaDon>, List<Billmodal>>(_data);

                // Duyệt qua từng hóa đơn để ánh xạ danh sách chi tiết hóa đơn
                foreach (var bill in _response)
                {
                    var hoadon = _data.FirstOrDefault(b => b.MaHoaDon == bill.MaHoaDon);
                    if (hoadon != null)
                    {
                        bill.DetailBills = hoadon.ChiTietHoaDons.Select(ct => new detailBillModal
                        {
                            MaVe = ct.MaVeNavigation.MaVe,   // Lấy mã vé từ MaVeNavigation
                            MaGhe = ct.MaVeNavigation.MaGhe  // Lấy mã ghế từ MaVeNavigation
                        }).ToList();
                    }
                }
            }

            return await Task.FromResult(_response);
        }
        // Phương thức lấy hóa đơn theo ID
        public async Task<Billmodal> GetByID(int id)
        {
            try
            {
                _logger.LogInformation("Fetching bill with ID: {BillId}", id);
                var bill = await _context.HoaDons.FindAsync(id);
                if (bill == null)
                {
                    _logger.LogWarning("Bill with ID {BillId} not found", id);
                    return null;
                }
                return _mapper.Map<Billmodal>(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill with ID: {BillId}", id);
                throw new Exception("An error occurred while fetching the bill.", ex);
            }
        }

        // Phương thức tạo hóa đơn mới
        public async Task<APIResponse> Create(Billmodal data)
        {
            var response = new APIResponse();
            try
            {
                if (data == null)
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Invalid data provided for creating a bill.";
                    _logger.LogWarning("Invalid data for creating bill");
                    return response;
                }

                var entity = _mapper.Map<HoaDon>(data);
                await _context.HoaDons.AddAsync(entity);
                await _context.SaveChangesAsync();

                response.ResponseCode = 201; // Created
                response.Result = "Tạo hóa đơn thành công!";
                _logger.LogInformation("Created new bill with ID: {BillId}", entity.MaHoaDon);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error creating bill");
            }

            return response;
        }

        // Phương thức xóa hóa đơn
        public async Task<APIResponse> Remove(int id)
        {
            var response = new APIResponse();
            try
            {
                var bill = await _context.HoaDons.FindAsync(id);
                if (bill != null)
                {
                    _context.HoaDons.Remove(bill);
                    await _context.SaveChangesAsync();
                    response.ResponseCode = 200; // OK
                    response.Result = "Hóa đơn đã được xóa!";
                    _logger.LogInformation("Bill with ID {BillId} deleted successfully", id);
                }
                else
                {
                    response.ResponseCode = 404; // Not Found
                    response.ErrorMessage = "Hóa đơn không tồn tại!";
                    _logger.LogWarning("Bill with ID {BillId} not found for deletion", id);
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error deleting bill with ID: {BillId}", id);
            }

            return response;
        }

        // Phương thức cập nhật hóa đơn
        public async Task<APIResponse> Update( int id)
        {
            var response = new APIResponse();
            try
            {
                // Tìm hóa đơn theo ID
                var bill = await _context.HoaDons.FindAsync(id);
                if (bill == null)
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Hóa đơn không tồn tại!";
                    return response;
                }

                if (bill.TinhTrang == 0)
                {
                    bill.TinhTrang = 1;
                }
                else
                {
                    bill.TinhTrang = 0;
                }


                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                response.ResponseCode = 200; // OK
                response.Result = "Cập nhật hóa đơn thành công!";
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error updating bill with ID: {BillId}", id);
            }

            return response;
        }
        public async Task<Billmodal> GetDetailsByID(int id)
        {
            try
            {
                // Lấy hóa đơn theo ID và bao gồm cả chi tiết hóa đơn
                var hoaDon = await _context.HoaDons
                    .Include(h => h.ChiTietHoaDons) // Bao gồm chi tiết hóa đơn
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    return null; // Nếu không tìm thấy, trả về null
                }

                // Ánh xạ dữ liệu sang Billmodal
                var bill = _mapper.Map<Billmodal>(hoaDon);
                bill.DetailBills = hoaDon.ChiTietHoaDons
                    .Select(ct => _mapper.Map<detailBillModal>(ct))
                    .ToList();

                return bill;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bill with ID: {BillId}", id);
                throw;
            }
        }
        public async Task<List<Billmodal>> GetBillsByCustomerId(string customerId)
        {
            try
            {
                var bills = await _context.HoaDons
                    .Where(h => h.MaKhachHang == customerId) // Tìm theo Mã Khách Hàng
                    .Include(h => h.ChiTietHoaDons)
                    .ToListAsync();

                var response = _mapper.Map<List<Billmodal>>(bills);

                // Ánh xạ chi tiết hóa đơn
                foreach (var bill in response)
                {
                    bill.DetailBills = bills
                        .FirstOrDefault(b => b.MaHoaDon == bill.MaHoaDon)?
                        .ChiTietHoaDons
                        .Select(ct => _mapper.Map<detailBillModal>(ct))
                        .ToList();
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bills for customer ID: {CustomerId}", customerId);
                throw;
            }
        }






        public async Task<List<BillHistoryModal>> GetUserBillHistory(string userId)
        {
            try
            {
                List<BillHistoryModal> model = new List<BillHistoryModal>();
                var ListBill = _context.HoaDons.Where(g=> g.MaKhachHang==userId).ToList();
                foreach(var item in ListBill)
                {
                   BillHistoryModal addto = new BillHistoryModal();
                   addto.MaHoaDon = item.MaHoaDon;
                   addto.TongTien = item.TongTien??0;
                   addto.TenUser = _context.TaiKhoans.Where(g=>g.IdtaiKhoan.Trim()==item.MaKhachHang.Trim()).Select(g=> g.TenNguoiDung).FirstOrDefault();
                   var Voucher = _context.Coupons.Where(g=>g.Id == item.MaGiamGia).FirstOrDefault();
                   var Percent = _context.GiamGia.Where(g=>g.MaGiamGia==Voucher.MaGiamGia).FirstOrDefault();
                   GiamGiaUsed discountInfo = new GiamGiaUsed();
                    discountInfo.MaNhap = Voucher.MaNhap;
                    discountInfo.Percent = Percent.GiaTri??0;
                    addto.GiamGia = discountInfo;
                   //var Percent = _context.GiamGia.Where(g=>g.MaGiamGia)
                    List<string> ListGhe = new List<string>();
                   var CTHD = _context.ChiTietHoaDons.Where(g => g.MaHoaDon==item.MaHoaDon).Select(h=>h.MaVe).ToList();
                   for(int i=0;i<CTHD.Count; i++)
                    {
                        var VeList = _context.Ves.Where(h => h.MaVe == CTHD[i]).FirstOrDefault();

                        ListGhe.Add(VeList.MaGhe);
                        if(i==0)
                        {
                            var Phim = _context.Phims.Where(g => g.Id == VeList.MaPhim).FirstOrDefault();
                            addto.TenPhim = Phim.TenPhim;
                           
                        }
                        addto.NgayTaoHoaDon = DateOnly.FromDateTime(VeList.ThoiGianTao??DateTime.Now);
                        //var LichChieu = _context.LichChieus.Wher
                        
                        var LichChieu = _context.LichChieus.Where(g => g.MaLichChieu == VeList.MaLichChieu).FirstOrDefault();
                        var GioChieu = _context.KhungGios.Where(g => g.Id == LichChieu.GioChieu).Select(g => g.GioBatDau).FirstOrDefault() ?? TimeOnly.FromDateTime(DateTime.Now);
                        DateOnly ngayChieuValue = LichChieu.NgayChieu ?? DateOnly.FromDateTime(DateTime.Now);
                        DateTime ngayGioChieu = ngayChieuValue.ToDateTime(GioChieu);
                        addto.GioDatPhim = ngayGioChieu;
                         
                    }
                    var CTHD1 = _context.ChiTietHoaDon1s.Where(g => g.MaHoaDon == item.MaHoaDon).ToList();
                    List<SanPhamDaMua> SanPhamDaMua = new List<SanPhamDaMua>(); 
                    for(int i=0; i<CTHD1.Count; i++)
                    {
                        SanPhamDaMua spBuyed = new SanPhamDaMua();
                        int soLuong = CTHD1[i].SoLuong ?? 0;
                        var SanPham = _context.SanPhams.Where(h => h.Id == CTHD1[i].MaSanPham).FirstOrDefault();
                        spBuyed.TenSanPham = SanPham.TenSanPham;
                        spBuyed.SoLuong = soLuong;
                        spBuyed.Gia = soLuong * (int)SanPham.Gia;
                        SanPhamDaMua.Add(spBuyed);
                    }
                    var CTHD2 = _context.ChiTietHoaDon2s.Where(g => g.MaHoaDon == item.MaHoaDon).ToList();
                    List<ComboDaMua> comboDaMuas = new List<ComboDaMua>();
                    for(int i=0; i<CTHD2.Count; i++)
                    {
                        ComboDaMua cbBuyed = new ComboDaMua();
                        int soLuong = CTHD2[i].SoLuong ?? 0;
                        var Combo = _context.Combos.Where(h => h.Id == CTHD2[i].MaCombo).FirstOrDefault();
                        cbBuyed.TenComBo = Combo.TenCombo;
                        cbBuyed.SoLuong = soLuong;
                        cbBuyed.Gia = soLuong * (int)Combo.Gia;
                        comboDaMuas.Add(cbBuyed);
                    }    
                addto.MaGhe = ListGhe;
                addto.SanPhamList = SanPhamDaMua;
                addto.ComboList = comboDaMuas;
                model.Add(addto);
                }    
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử hóa đơn của user ID: {UserId}", userId);
                throw;
            }
        }
    }
}
