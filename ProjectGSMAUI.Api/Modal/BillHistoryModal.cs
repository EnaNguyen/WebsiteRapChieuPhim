namespace ProjectGSMAUI.Api.Modal
{
    public class BillHistoryModal
    {
        public string TenPhim { get; set; }    // Tên phim
        public int MaHoaDon { get; set; }    // Tên phim
        public int MaPhim { get; set; }    // Tên phim
        public DateTime GioDatPhim { get; set; } // Giờ đặt phim
        public DateOnly NgayDatPhim { get; set; } // Ngày đặt phim
        public string TenUser { get; set; }    // Tên user
        public List<string> MaGhe { get; set; }      // Mã ghế
    }


}
