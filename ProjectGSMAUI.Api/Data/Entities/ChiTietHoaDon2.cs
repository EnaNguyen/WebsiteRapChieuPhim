namespace ProjectGSMAUI.Api.Data.Entities
{
    public class ChiTietHoaDon2
    {
        public int MaChiTietHoaDon { get; set; }

        public int? MaCombo { get; set; }

        public int? MaHoaDon { get; set; }

        public int? SoLuong { get; set; }
        public int? Gia { get; set; }

        public virtual Combo? ComboNavigation { get; set; }

        public virtual HoaDon? MaHoaDonNavigation { get; set; }
    }
}
