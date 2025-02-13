using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMVC.Models
{
    public class PhimVM
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
        public byte[]? HinhAnh { get; set; }

        public string? Video { get;set; }
        public List<HinhAnh> HinhAnhs { get; set; }
        
    }
}
