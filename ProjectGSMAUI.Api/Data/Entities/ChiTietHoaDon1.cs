namespace ProjectGSMAUI.Api.Data.Entities
{
    public class ChiTietHoaDon1
    {
        public int MaChiTietHoaDon { get; set; }

        public int? MaSanPham { get; set; }

        public int? MaHoaDon { get; set; }

        public int? SoLuong { get; set; }
        public int? Gia { get; set; }

        public virtual SanPham? SanPhamNavigation { get; set; }

        public virtual HoaDon? MaHoaDonNavigation { get; set; }
    }
}
