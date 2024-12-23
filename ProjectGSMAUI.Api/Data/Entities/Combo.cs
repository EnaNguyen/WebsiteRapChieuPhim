using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.Api.Data.Entities
{
    public class Combo
    {
        [Key]
        public int Id { get; set; }
        public string TenCombo { get; set; } 
        public decimal Gia { get; set; } 
        public string MoTa { get; set; } 
        public string HinhAnh { get; set; }
        public ICollection<ChiTietCombo> ChiTietCombos { get; set; } = new List<ChiTietCombo>();
    }
}
