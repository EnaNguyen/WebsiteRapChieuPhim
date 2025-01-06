using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectGSMAUI.Api.Container
{
    public class GiamGiaServices: IGiamGiaServices
    {
        private readonly ApplicationDbContext context;
		private readonly IMapper mapper;
		private readonly ILogger<GiamGiaServices> logger;
		public GiamGiaServices(ApplicationDbContext _context, IMapper mapper, ILogger<GiamGiaServices> _logger)
        {
			context = _context;
			this.mapper = mapper;
			logger = _logger;
		}

        public async Task<APIResponse> Create(GiamGiaRequest data)
        {
            APIResponse response = new APIResponse();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Validate ngày bắt đầu
                if (data.NgayBatDau < DateOnly.FromDateTime(DateTime.Today))
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Ngày bắt đầu phải từ hôm nay trở về sau.";
                    return response;
                }

                // Validate ngày bắt đầu và ngày kết thúc
                if (data.NgayBatDau > data.NgayKetThuc)
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc.";
                    return response;
                }

                // Validate giá trị
                if (data.GiaTri <= 0 || data.GiaTri > 100)
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Giá trị phải lớn hơn 0 và nhỏ hơn hoặc bằng 100.";
                    return response;
                }

                // Validate số lượng
                if (data.SoLuong <= 0)
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Số lượng phải lớn hơn 0.";
                    return response;
                }

                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(data.ImageFile))
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(data.ImageFile);
                    }
                    catch (FormatException)
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                        return response;
                    }
                }

                // Tạo mã giảm giá mới
                int maxMaGiamGia = await context.GiamGia
                    .AsNoTracking()
                    .OrderByDescending(g => g.MaGiamGia)
                    .Select(g => g.MaGiamGia)
                    .FirstOrDefaultAsync();
                maxMaGiamGia += 1;

                var giamGia = new GiamGia
                {
                    MaGiamGia = maxMaGiamGia,
                    TenGiamGia = data.TenGiamGia,
                    NgayBatDau = data.NgayBatDau,
                    NgayKetThuc = data.NgayKetThuc,
                    MoTa = data.MoTa,
                    GiaTri = data.GiaTri,
                    SoLuong = data.SoLuong,
                    HinhAnh = imageBytes, // Lưu byte[] vào CSDL
                };

                // Lưu dữ liệu giảm giá
                this.logger.LogInformation("Create Begins");
                await this.context.AddAsync(giamGia);
                await this.context.SaveChangesAsync();

                // Tạo mã coupon
                int maxCouponId = await context.Coupons
                    .AsNoTracking()
                    .OrderByDescending(c => c.Id)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                maxCouponId = maxCouponId + 1;
                List<Coupon> coupons = new List<Coupon>();
                for (int i = 0; i < data.SoLuong; i++)
                {
                    string uniqueCode;
                    do
                    {
                        uniqueCode = GenerateUniqueCode();
                    }
                    while (await context.Coupons.AnyAsync(c => c.MaNhap == uniqueCode));

                    coupons.Add(new Coupon
                    {
                        Id = maxCouponId + i,
                        MaNhap = uniqueCode,
                        MaGiamGia = maxMaGiamGia,
                        TrangThai = true
                    });
                }

                await context.Coupons.AddRangeAsync(coupons);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                response.ResponseCode = 201;
                response.Result = maxCouponId.ToString();
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
                this.logger.LogError(ex.Message, ex);
            }
            return response;
        }



        public async Task<List<ActiveGiamGia>> GetAll(string Name)
		{
            
			List<ActiveGiamGia> _response = new List<ActiveGiamGia>();
            if(Name== null)
            {
                var _data = await this.context.GiamGia
                .Include(g => g.Coupons)
                .ToListAsync();
                if (_data != null)
                {
                    _response = this.mapper.Map<List<GiamGia>, List<ActiveGiamGia>>(_data);
                }
            }    	
            else
            {
                var _data = await this.context.GiamGia.Include(g => g.Coupons).Where(a => a.TenGiamGia.Contains(Name)).ToListAsync();
                if (_data != null)
                {
                    _response = this.mapper.Map<List<GiamGia>, List<ActiveGiamGia>>(_data);
                    if (_data != null)
                    {
                        _response = this.mapper.Map<List<GiamGia>, List<ActiveGiamGia>>(_data);
                    }
                }
            }
			return _response;
		}
		public async Task<ActiveGiamGia> GetByID(int id)
		{
			ActiveGiamGia _response = new ActiveGiamGia();
			var _data = await this.context.GiamGia
				.Include(g => g.Coupons)
				.FirstOrDefaultAsync(g => g.MaGiamGia == id); 

			if (_data != null)
			{
				_response = this.mapper.Map<GiamGia, ActiveGiamGia>(_data);
                
			}
			return _response;
		}
		public async Task<APIResponse> Remove(int id)
		{
			APIResponse response = new APIResponse();
			using var transaction = await this.context.Database.BeginTransactionAsync();
			try
			{
				var maGiamGia = await this.context.GiamGia
					.Include(g => g.Coupons) 
					.FirstOrDefaultAsync(g => g.MaGiamGia == id);

				if (maGiamGia != null)
				{
					this.context.Coupons.RemoveRange(maGiamGia.Coupons);
					this.context.GiamGia.Remove(maGiamGia);
					await this.context.SaveChangesAsync();
					await transaction.CommitAsync();
					response.ResponseCode = 201;
					response.Result = id.ToString();
				}
				else
				{
					response.ResponseCode = 404;
					response.ErrorMessage = "Không tìm thấy Voucher tương ứng!";
				}
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();

				response.ResponseCode = 400;
				response.ErrorMessage = ex.Message;
			}
			return response;
		}


		public async Task<APIResponse> Update(GiamGiaRequest data, int id)
		{
			APIResponse response = new APIResponse();
			try
			{
				if (data.GiaTri <= 0 || data.GiaTri >= 100)
				{
					response.ResponseCode = 400;
					response.ErrorMessage = "Giá trị phải lớn hơn 0 và nhỏ hơn 100.";
					return response;
				}
				if (data.NgayKetThuc <= data.NgayBatDau)
				{
					response.ResponseCode = 400;
					response.ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu.";
					return response;
				}
				var giamGia = await this.context.GiamGia.FindAsync(id);
				if (giamGia == null)
				{
					response.ResponseCode = 404;
					response.ErrorMessage = "Không tìm thấy GiamGia tương ứng!";
					return response;
				}
				if (data.SoLuong != giamGia.SoLuong)
				{
					response.ResponseCode = 404;
					response.ErrorMessage = "Không cho phép thay đổi số lượng Mã Giảm Giá, Hãy Thêm hoặc Xóa mã Giảm Giá!";
					return response;
				}
                byte[] imageBytes = null;
                if (!string.IsNullOrEmpty(data.ImageFile))
                {
                    try
                    {
                        imageBytes = Convert.FromBase64String(data.ImageFile);
                    }
                    catch (FormatException)
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Dữ liệu hình ảnh không hợp lệ.";
                        return response;
                    }
                }


				giamGia.TenGiamGia = data.TenGiamGia;
				giamGia.GiaTri = data.GiaTri;
				giamGia.NgayBatDau = data.NgayBatDau;
				giamGia.NgayKetThuc = data.NgayKetThuc;
				giamGia.MoTa = data.MoTa;
				if (data.ImageFile != null)
					giamGia.HinhAnh = imageBytes;
				this.context.GiamGia.Update(giamGia);
				await this.context.SaveChangesAsync();

				response.ResponseCode = 200;
				response.Result = "Cập nhật thành công!";
			}
			catch (Exception ex)
			{
				response.ResponseCode = 400;
				response.ErrorMessage = ex.Message;
			}
			return response;
		}

		private string GenerateUniqueCode()
		{
			return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
		}
	}
}
