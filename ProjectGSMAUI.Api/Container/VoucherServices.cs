using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectGSMAUI.Api.Container
{
    public class VoucherServices : IVoucherServices
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<VoucherServices> logger;
        public VoucherServices(ApplicationDbContext _context, IMapper mapper, ILogger<VoucherServices> logger)
        {
            context = _context;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<APIResponse> Create(ActiveVoucher data)
        {
            APIResponse response = new APIResponse();
            try
            {
                this.logger.LogInformation("Create Begins");
                Coupon _coupon = this.mapper.Map<ActiveVoucher, Coupon>(data);
                await this.context.Coupons.AddAsync(_coupon);
                await this.context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = data.Id.ToString();
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
                this.logger.LogError(ex.Message, ex);
            }
            return response;
        }

        public async Task<List<ActiveVoucher>> GetAll()
        {
            List<ActiveVoucher> _response = new List<ActiveVoucher>();
            var _data = await this.context.Coupons.ToListAsync();
            if (_data != null)
            {
                _response = this.mapper.Map<List<Coupon>, List<ActiveVoucher>>(_data);
            }
            return await Task.FromResult(_response);
        }

        public async Task<List<Coupon>> GetByGiamGia(int Id)
        {
            var data = await this.context.Coupons.Where(a => a.MaGiamGia == Id).ToListAsync();
            return data;
        }
        public async Task<ActiveVoucher> GetByID(int id)
        {
            ActiveVoucher _response = new ActiveVoucher();
            var _data = await this.context.Coupons.FindAsync(id);
            if (_data != null)
            {
                _response = this.mapper.Map<Coupon,ActiveVoucher>(_data);
            }
            return _response;
        }

		public async Task<APIResponse> Remove(int id)
		{
			APIResponse response = new APIResponse();
			using var transaction = await this.context.Database.BeginTransactionAsync();
			try
			{
				var coupon = await this.context.Coupons
					.Include(c => c.MaGiamGiaNavigation) 
					.FirstOrDefaultAsync(c => c.Id == id);

				if (coupon != null)
				{
					var giamGia = coupon.MaGiamGiaNavigation;
					if (giamGia != null)
					{
						giamGia.SoLuong -= 1;
						this.context.GiamGia.Update(giamGia); 
					}
					this.context.Coupons.Remove(coupon);
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

        public async Task<APIResponse> StatusUpdate(string ma)
        {
            APIResponse response = new APIResponse();
            using var transaction = await this.context.Database.BeginTransactionAsync();
            try
            {

                var Voucher = context.Coupons.Where(g => g.MaNhap.Trim().ToLower() == ma.Trim().ToLower()).FirstOrDefault();
                if (Voucher != null && Voucher.TrangThai == true)
                {
                    Voucher.TrangThai = false;
                    this.context.Update(Voucher);
                    await this.context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.ResponseCode = 200;
                }
                else
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Mã Đã Dùng";
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

        public async Task<APIResponse> Update(ActiveVoucher data, int id)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _coupon = await this.context.Coupons.FindAsync(id);
                if (_coupon != null)
                {
                   _coupon.TrangThai = data.TrangThai;
                   await this.context.SaveChangesAsync();
                }
                else
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Không tìm thấy Voucher tương ứng!";
                }
                response.ResponseCode = 201;
                response.Result = id.ToString();
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public async Task<APIResponse> Used(string ma)
        {
            APIResponse response = new APIResponse();
            using var transaction = await this.context.Database.BeginTransactionAsync();
            try
            {

                var Voucher = context.Coupons.Where(g => g.MaNhap.Trim().ToLower() == ma.Trim().ToLower()).FirstOrDefault();
                if(Voucher!=null && Voucher.TrangThai==true)
                {
                    int Percent = context.GiamGia.Where(g => g.MaGiamGia == Voucher.MaGiamGia).Select(g => g.GiaTri).FirstOrDefault() ?? 0;
                    await this.context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.ResponseCode = 200;
                    response.Result = Percent.ToString();
                    response.Data = Voucher.Id;
                }    
                else
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Mã Đã Dùng";
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
    }
}
