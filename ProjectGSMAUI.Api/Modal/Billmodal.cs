using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Modal
{
    public class Billmodal
    {
        public int MaHoaDon { get; set; }

        public int? MaDatVe { get; set; }

        public string? MaKhachHang { get; set; }

        public int? TongTien { get; set; }

        public int? MaGiamGia { get; set; }

        public DateOnly? NgayXuat { get; set; }

        public string? TinhTrangDisplay { get; set; }

        public List<detailBillModal> DetailBills { get; set; }

    }
}
