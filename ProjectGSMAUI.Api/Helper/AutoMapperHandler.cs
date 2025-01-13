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



            // Mapping cho HoaDon và Billmodal
            CreateMap<HoaDon, Billmodal>()
                .ForMember(dest => dest.TinhTrangDisplay, opt => opt.MapFrom(src =>
                    src.TinhTrang == 1 ? "Đã thanh toán" : "Chưa thanh toán"))
                .ReverseMap()
                .ForMember(dest => dest.TinhTrang, opt => opt.MapFrom(src =>
                    src.TinhTrangDisplay != null && src.TinhTrangDisplay.ToLower() == "đã thanh toán" ? 1 : 0));

            // Mapping cho ChiTietHoaDon và detailBillModal
            CreateMap<ChiTietHoaDon, detailBillModal>()
                 .ReverseMap();

        }
    }
}
