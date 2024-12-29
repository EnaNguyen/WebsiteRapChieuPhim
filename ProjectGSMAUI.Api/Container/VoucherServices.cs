using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectGSMAUI.Api.Container
{
    public class VoucherServices : IVoucherServices
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public VoucherServices(ApplicationDbContext _context, IMapper mapper)
        {
            context = _context;
            this.mapper = mapper;
        }

        public async Task<APIResponse> Create(ActiveVoucher data)
        {
            APIResponse response = new APIResponse();
            try
            {
                Coupon _coupon = this.mapper.Map<ActiveVoucher,Coupon>(data);
                await this.context.Coupons.AddAsync(_coupon);
                await this.context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = data.Id.ToString();
            }
            catch (Exception ex) 
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
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
            try
            {
                var _customer = await this.context.Coupons.FindAsync(id);
                if (_customer != null)
                {
                    this.context.Coupons.Remove(_customer);
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
    }
}
