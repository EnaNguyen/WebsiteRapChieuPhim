using AutoMapper;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Modal;
using System.Runtime;

namespace ProjectGSMAUI.Api.Helper
{
    public class AutoMapperHandler:Profile
    {
        public AutoMapperHandler() 
        {
            CreateMap<Coupon, ActiveVoucher>().ForMember(item => item.TenTrangThai, opt => opt.MapFrom(item =>
            (item.TrangThai!=null && item.TrangThai.Value)?"Còn lại":"Đã sử dụng")).ReverseMap();
        }
    }
}
