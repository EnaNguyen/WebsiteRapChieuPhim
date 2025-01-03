using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Services
{
    public class TheLoaiPhimService : ITheLoaiPhimService
    {
        private readonly ApplicationDbContext context;
        public TheLoaiPhimService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<APIResponse> GetAll()
        {
            try
            {
                var data = context.TheLoaiPhims.ToList();
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Danh sách thể loại phim",
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

        public async Task<TheLoaiPhim> GetByID(int id)
        {
            return await context.TheLoaiPhims.FindAsync(id);
        }
        public async Task<APIResponse> Create(TheLoaiPhim data)
        {
            try
            {
                context.TheLoaiPhims.Add(data);
                await context.SaveChangesAsync();
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Tạo thành công!",
                    ErrorMessage = null
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

        public async Task<APIResponse> Update(TheLoaiPhim data, int id)
        {
            var existing = await context.TheLoaiPhims.FindAsync(id);
            if (existing == null)
            {
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = null,
                    ErrorMessage = "Không tìm thấy thể loại phim."
                };
            }

            try
            {
                existing.TenTheLoai = data.TenTheLoai;
                context.TheLoaiPhims.Update(existing);
                await context.SaveChangesAsync();
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Cập nhật thành công!",
                    ErrorMessage = null
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

        public async Task<APIResponse> Remove(int id)
        {
            var existing = await context.TheLoaiPhims.FindAsync(id);
            if (existing == null)
            {
                return new APIResponse
                {
                    ResponseCode = 404,
                    Result = null,
                    ErrorMessage = "Không tìm thấy thể loại phim."
                };
            }

            try
            {
                context.TheLoaiPhims.Remove(existing);
                await context.SaveChangesAsync();
                return new APIResponse
                {
                    ResponseCode = 200,
                    Result = "Xóa thành công!",
                    ErrorMessage = null
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