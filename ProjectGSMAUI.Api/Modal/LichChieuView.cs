namespace ProjectGSMAUI.Api.Modal
{
    public class LichChieuView
    {
        public int MaLichChieu { get; set; }

        public DateOnly? NgayChieu { get; set; }
        public int MaGioChieu { get; set; }

        public TimeOnly? GioChieu { get; set; }

        public int? MaPhim { get; set; }

        public int? MaPhong { get; set; }

        public decimal? GiaVe { get; set; }
    }
}
