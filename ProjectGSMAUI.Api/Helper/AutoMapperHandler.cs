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
            (item.TrangThai!=null && item.TrangThai.Value)?"Chưa Sử dụng":"Đã sử dụng")).ReverseMap();
			CreateMap<GiamGia, ActiveGiamGia>()
			   .ForMember(
				   dest => dest.SoLuongConLai,
				   opt => opt.MapFrom(src =>
					   src.SoLuong.HasValue
					   ? src.SoLuong.Value - src.Coupons.Count(c => c.TrangThai == false)
					   : 0
				   )
			   )
			   .ForMember(
				   dest => dest.SoLuongDaDung,
				   opt => opt.MapFrom(src =>
					   src.Coupons.Count(c => c.TrangThai == false)
				   )
			   )
			   .ReverseMap();
			 int i = 1;
		}
    }
}
