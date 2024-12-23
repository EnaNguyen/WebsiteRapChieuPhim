using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.Api.Data.Entities
{
    public class ChiTietCombo
    {
        [Key]
        public int Id { get; set; }
        public int ComboId { get; set; }
        public Combo Combo { get; set; } = null!;
        public int SanPhamId { get; set; }
        public SanPham SanPham { get; set; } = null!;
        public int SoLuong { get; set; } 
    }
}
