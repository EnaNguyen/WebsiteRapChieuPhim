using ProjectGSMAUI.Api.Data.Entities;
namespace ProjectGSMAUI.Api.Modal
{
    public class DatVeModel
    {
        public int MaPhim { get; set; }
        public DateOnly? NgayChieu { get; set; }
        public int CaChieu { get; set; }
        public List<string> Ghe { get; set; }
        public List<SanPhamBuying>? SanPham { get; set; }
        public List<ComboBuying>? Combo {  get; set; }
        public int? TongTien { get; set; }
        public string? Ma { get; set; }
    }
}
