using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.Api.Data.Entities
{
    public class ChiTietCombo
    {
        [Key]
        public int Id { get; set; }
        public int ComboId { get; set; }
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }

        // Thêm navigation properties
        public Combo Combo { get; set; }
        public SanPham SanPham { get; set; }
    }
}
