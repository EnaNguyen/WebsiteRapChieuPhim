namespace ProjectGSMAUI.Api.Modal
{
    public class ComboModal
    {
        public string TenCombo { get; set; }
        public decimal Gia { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
        public List<ChiTietComboModal> ChiTietCombos { get; set; } = new List<ChiTietComboModal>();
    }
}
