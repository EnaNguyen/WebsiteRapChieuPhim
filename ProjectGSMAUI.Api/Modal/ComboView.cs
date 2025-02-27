namespace ProjectGSMAUI.Api.Modal
{
    public class ComboView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SanPhamInCombo> sanPhams { get; set; }
        public int Gia { get; set; }
        public string Hinh { get; set; }
    }
    public class SanPhamInCombo
    {
        public string Name { get; set; }
        public int SoLuong { get; set; }
    }
}
