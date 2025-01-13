using ProjectGSMVC.Areas.Admin.Controllers;

namespace ProjectGSMVC.Areas.Admin.Models
{
    public class LichChieuViewModel
    {
        public int MaLichChieu { get; set; }
        public DateOnly? NgayChieu { get; set; }
        public string GioChieu { get; set; }
        public int? MaPhim { get; set; }
        public int? MaPhong { get; set; }
        public decimal? GiaVe { get; set; }
        public bool? TinhTrang { get; set; }
        public string? TenPhim { get; set; }
        public string? TenPhong { get; set; }
        public string? ThoiGianChieu { get; set; }
    }
  
}
