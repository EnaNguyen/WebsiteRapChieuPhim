namespace ProjectGSMAUI.Api.Modal
{
    public class CreatePhim
    {
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
    }
}
