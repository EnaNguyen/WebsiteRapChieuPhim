using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMVC.Areas.Admin.Models
{
    public class PhimViewModel
    {
        public int Id { get; set; }

        public string? TenPhim { get; set; }

        public string? TheLoai { get; set; }

        public int? ThoiLuong { get; set; }

        public string? DaoDien { get; set; }

        public int? GioiHanDoTuoi { get; set; }

        public DateOnly? NgayKhoiChieu { get; set; }

        public DateOnly? NgayKetThuc { get; set; }

        public int? SoSuatChieu { get; set; }

        public int? TrangThai { get; set; }

        public string? MoTa { get; set; }
        public IFormFile ImageFile { get; set; } // Đổi kiểu từ string thành IFormFile
        public string? ImageBase64 { get; set; } // Thuộc tính này để lưu hình ảnh dưới dạng Base64 nếu cần

        public virtual ICollection<HinhAnh> HinhAnhs { get; set; } = new List<HinhAnh>();

        public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();

        public virtual TheLoaiPhim? TheLoaiNavigation { get; set; }

        public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
        
    }
}
