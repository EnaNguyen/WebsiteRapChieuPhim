namespace ProjectGSMAUI.Api.Modal
{
    public class ComboCreate
    {
        public string TenCombo { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
        public List<SanPhamList>? ChiTietCombos { get; set; } = new List<SanPhamList>();

    }
    public class SanPhamList
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }    
    }
}
