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

        // Phương thức lấy tất cả hóa đơn
        //public async Task<List<Billmodal>> GetAll()
        //{

        //        List<Billmodal> _response = new List<Billmodal>();
        //    var _data = await this._context.HoaDons.ToListAsync();
        //    if (_data != null)
        //    {
        //        _response = this._mapper.Map<List<HoaDon>, List<Billmodal>>(_data);
        //    }
        //    return await Task.FromResult(_response);

        //}
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

        public async Task<List<BillHistoryModal>> GetUserBillHistory(string userId)
        {
            try
            {
                var userBills = await _context.HoaDons
                    .Include(h => h.ChiTietHoaDons) // ✅ JOIN `ChiTietHoaDon`
                    .ThenInclude(ct => ct.MaVeNavigation) // ✅ JOIN `Ve`
                    .Include(h => h.MaKhachHangNavigation) // ✅ JOIN `TaiKhoan`
                    .Where(h => h.MaKhachHang == userId)
                    .ToListAsync();

                if (userBills == null || userBills.Count == 0)
                {
                    return new List<BillHistoryModal>();
                }

                var billHistory = userBills.Select(h => new BillHistoryModal
                {
                    MaHoaDon = h.MaHoaDon,

                    // ✅ Lấy `MaVe` từ `ChiTietHoaDon`
                    MaVe = h.ChiTietHoaDons
                        .Where(ct => ct.MaVeNavigation != null)
                        .Select(ct => ct.MaVeNavigation.MaVe)
                        .FirstOrDefault() ?? "Không có dữ liệu",

                    // ✅ Lấy ngày xuất hóa đơn
                    NgayDatPhim = h.NgayXuat.HasValue ? h.NgayXuat.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,

                    // ✅ Lấy mã ghế từ `Ve`
                    MaGhe = h.ChiTietHoaDons
                .Select(ct => ct.MaVeNavigation?.MaGhe)
                .FirstOrDefault() ?? "Không có dữ liệu",
                    // ✅ Lấy tên người dùng
                    TenUser = h.MaKhachHangNavigation?.TenNguoiDung ?? "Không có dữ liệu"

                }).ToList();

                return billHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử hóa đơn của user ID: {UserId}", userId);
                throw;
            }
        }

    }
}
