namespace ProjectGSMAUI.Api.Modal
{
    public class SanPhamBuying
    {
        public int Id { get; set; }
        public int? MaHoaDon {  get; set; }
        public int quantity { get; set; }
        public int? DonGia { get; set; }
    }
    public class ComboBuying
    {
        public int Id { get; set; }
        public int? MaHoaDon { get; set; }
        public int quantity { get; set; }
        public int? DonGia { get; set; }
    }
}
