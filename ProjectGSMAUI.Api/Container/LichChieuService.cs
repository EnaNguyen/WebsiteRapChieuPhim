using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper; // Đảm bảo đã thêm dòng này
using ProjectGSMAUI.Api.Modal;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public class LichChieuService : ILichChieuService
    {
        private readonly ApplicationDbContext context;

        public LichChieuService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<APIResponse> GetAll()
        {
            try
            {
                // Truy vấn dữ liệu và lấy các thông tin chi tiết từ bảng liên quan
                var data = await context.LichChieus
                    .Include(l => l.MaPhimNavigation) // Lấy dữ liệu từ bảng Phim
                    .Include(l => l.MaPhongNavigation) // Lấy dữ liệu từ bảng Phòng
                    .Include(l => l.GioChieuNavigation) // Lấy dữ liệu từ bảng KhungGio
                    .Select(l => new
                    {
                        MaLichChieu = l.MaLichChieu,
                        TenPhim = l.MaPhimNavigation.TenPhim, // Lấy tên phim
                        TenPhong = l.MaPhongNavigation.TenPhong, // Lấy tên phòng
                        NgayChieu = l.NgayChieu, // Ngày chiếu
                        GioChieu = l.GioChieuNavigation.GioBatDau, // Giờ chiếu từ bảng KhungGio
                        GiaVe = l.GiaVe, // Giá vé
                        TinhTrang = l.TinhTrang // Trạng thái
                    })
                    .ToListAsync();

                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Danh sách lịch chiếu phim",
                    ErrorMessage = null,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = null,
                    ErrorMessage = ex.Message
                };
            }
        }


        public async Task<LichChieu> GetByID(int id)
        {
            return await context.LichChieus.FindAsync(id);
        }

        public async Task<APIResponse> GenerateSchedule()
        {
            try
            {
                // Lấy dữ liệu từ cơ sở dữ liệu
                var films = await context.Phims.Where(f => f.TrangThai == 1).ToListAsync(); // Lấy danh sách phim có trạng thái '1' (đang chiếu)
                var rooms = await context.Phongs.ToListAsync(); // Lấy danh sách phòng chiếu
                var timeSlots = await context.KhungGios.ToListAsync(); // Lấy danh sách khung giờ chiếu

                var scheduler = new LichTrinhRapChieuPhim(); // Tạo đối tượng lịch trình chiếu
                var schedule = scheduler.TaoLichChieu(films, rooms, timeSlots, out var phimDaCapNhat); // Tạo lịch chiếu

                // Thêm lịch chiếu vào cơ sở dữ liệu
                context.LichChieus.AddRange(schedule);

                // Cập nhật thông tin phim (số suất chiếu và trạng thái phim)
                context.Phims.UpdateRange(phimDaCapNhat);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await context.SaveChangesAsync();

                // Trả về API response
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Lịch chiếu phim được tạo thành công",
                    ErrorMessage = null,
                    Data = schedule // Trả về lịch chiếu vừa tạo
                };
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, trả về thông báo lỗi
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = null,
                    ErrorMessage = ex.Message
                };
            }
        }


        public async Task<APIResponse> AddSchedule(LichChieu schedule)
        {
            try
            {
                // Tăng mã lịch chiếu
                var maxCode = await context.LichChieus.MaxAsync(l => (int?)l.MaLichChieu) ?? 0;
                schedule.MaLichChieu = maxCode + 1;

                await context.LichChieus.AddAsync(schedule);
                await context.SaveChangesAsync();

                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Lịch chiếu phim được thêm thành công",
                    ErrorMessage = null,
                    Data = schedule
                };
            }
            catch (DbUpdateException dbEx)
            {
                // Log chi tiết lỗi
                Console.Error.WriteLine("Lỗi khi lưu dữ liệu: " + dbEx.InnerException?.Message);

                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = null,
                    ErrorMessage = "Lỗi cơ sở dữ liệu: " + dbEx.InnerException?.Message
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = null,
                    ErrorMessage = "Lỗi không mong muốn: " + ex.Message
                };
            }
        }



        public async Task<APIResponse> DeleteSchedule(int id)
        {
            try
            {
                var schedule = await context.LichChieus.FindAsync(id);
                if (schedule == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = null,
                        ErrorMessage = "Không tìm thấy lịch chiếu phim."
                    };
                }
                context.LichChieus.Remove(schedule);
                await context.SaveChangesAsync();

                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Lịch chiếu phim đã được xóa thành công",
                    ErrorMessage = null,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = null,
                    ErrorMessage = ex.Message
                };
            }
        }

        // Phương thức mới
        public async Task<APIResponse> GenerateScheduleWithAdjustments(int phimCanTangId, int soSuatCanTang)
        {
            try
            {
                var films = await context.Phims.ToListAsync(); 
                var rooms = await context.Phongs.ToListAsync();
                var timeSlots = await context.KhungGios.ToListAsync();

                var phimCanTang = films.FirstOrDefault(f => f.Id == phimCanTangId);
                if (phimCanTang == null)
                {
                    return new APIResponse
                    {
                        ResponseCode = 404,
                        Result = null,
                        ErrorMessage = "Không tìm thấy phim cần tăng suất chiếu."
                    };
                }

                // Kiểm tra nếu phim không phải phim đang chiếu (phim có trạng thái khác 1)
                if (phimCanTang.TrangThai == 1)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = null,
                        ErrorMessage = "Phim không đang chiếu hoặc trạng thái phim không hợp lệ."
                    };
                }

                // Tính tổng số suất chiếu hiện tại và tính toán lại số suất chiếu
                int tongSuatHienTai = films.Where(f => f.TrangThai == 1).Sum(f => f.SoSuatChieu ?? 0); // Chỉ tính suất chiếu của các phim đang chiếu
                int tongSuatMoi = tongSuatHienTai + soSuatCanTang;

                if (tongSuatMoi > LichTrinhRapChieuPhim.TongSuat)
                {
                    return new APIResponse
                    {
                        ResponseCode = 400,
                        Result = null,
                        ErrorMessage = "Số suất chiếu tổng vượt quá giới hạn của rạp."
                    };
                }

                // Điều chỉnh số suất chiếu của phim cần tăng
                phimCanTang.SoSuatChieu += soSuatCanTang;

                // Điều chỉnh số suất chiếu của các phim còn lại
                int suatGiamMoiPhim = soSuatCanTang / (films.Count(f => f.TrangThai == 1) - 1); // Chỉ giảm suất chiếu của các phim đang chiếu
                foreach (var phim in films)
                {
                    if (phim.Id != phimCanTangId && phim.TrangThai == 1)
                    {
                        phim.SoSuatChieu = Math.Max((phim.SoSuatChieu ?? 0) - suatGiamMoiPhim, 0);
                        if (phim.SoSuatChieu == 0)
                        {
                            phim.TrangThai = 0;
                        }
                    }
                }

                // Cập nhật số lượng suất chiếu và trạng thái phim trong cơ sở dữ liệu
                context.Phims.UpdateRange(films);
                await context.SaveChangesAsync();

                // Tạo lịch chiếu mới
                var scheduler = new LichTrinhRapChieuPhim();
                var schedule = scheduler.TaoLichChieu(films, rooms, timeSlots, out var phimDaCapNhat);

                context.LichChieus.AddRange(schedule);
                context.Phims.UpdateRange(phimDaCapNhat);
                await context.SaveChangesAsync();

                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Lịch chiếu phim được tạo thành công với điều chỉnh",
                    ErrorMessage = null,
                    Data = schedule
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    ResponseCode = 500,
                    Result = null,
                    ErrorMessage = ex.Message
                };
            }
        }

    }
}
