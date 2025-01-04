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
                var data = await context.LichChieus.ToListAsync();
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
                var films = await context.Phims.Where(f => f.TrangThai == 1).ToListAsync();
                var rooms = await context.Phongs.ToListAsync();
                var timeSlots = await context.KhungGios.ToListAsync();

                var scheduler = new LichTrinhRapChieuPhim(); // Đã đổi tên lớp
                var schedule = scheduler.TaoLichChieu(films, rooms, timeSlots, out var phimDaCapNhat);

                context.LichChieus.AddRange(schedule);
                context.Phims.UpdateRange(phimDaCapNhat); // Cập nhật số suất chiếu và trạng thái phim trong cơ sở dữ liệu
                await context.SaveChangesAsync();

                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Lịch chiếu phim được tạo thành công",
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

        public async Task<APIResponse> AddSchedule(LichChieu schedule)
        {
            try
            {
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
                var films = await context.Phims.Where(f => f.TrangThai == 1).ToListAsync();
                var rooms = await context.Phongs.ToListAsync();
                var timeSlots = await context.KhungGios.ToListAsync();

                // Tìm phim cần tăng suất chiếu
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

                // Tính tổng số suất chiếu hiện tại và tính toán lại số suất chiếu
                int tongSuatHienTai = films.Sum(f => f.SoSuatChieu ?? 0);
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
                int suatGiamMoiPhim = soSuatCanTang / (films.Count - 1);
                foreach (var phim in films)
                {
                    if (phim.Id != phimCanTangId)
                    {
                        phim.SoSuatChieu = Math.Max((phim.SoSuatChieu ?? 0) - suatGiamMoiPhim, 0);
                        if (phim.SoSuatChieu == 0)
                        {
                            phim.TrangThai = 0; // Chuyển trạng thái phim thành hết phim
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
                context.Phims.UpdateRange(phimDaCapNhat); // Cập nhật số suất chiếu và trạng thái phim sau khi lập lịch
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
