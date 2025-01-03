using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
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

                var scheduler = new CinemaScheduler();
                var schedule = scheduler.GenerateSchedule(films, rooms, timeSlots);

                context.LichChieus.AddRange(schedule);
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
    }
}
