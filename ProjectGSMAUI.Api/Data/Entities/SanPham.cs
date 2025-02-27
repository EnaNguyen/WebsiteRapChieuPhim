using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.Api.Data.Entities
{
    public class SanPham
    {
        [Key]
        public int Id { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public decimal Gia { get; set; }

        public string MoTa { get; set; } = string.Empty;

        public int SoLuong { get; set; }

        public byte[]? HinhAnh { get; set; }

        public ICollection<ChiTietCombo> ChiTietCombos { get; set; } = new List<ChiTietCombo>();
        public virtual ICollection<ChiTietHoaDon1> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon1>();

    }
}
