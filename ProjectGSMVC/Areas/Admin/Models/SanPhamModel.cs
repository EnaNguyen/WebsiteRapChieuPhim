using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.MVC.Models
{
    public class SanPhamModel
    {
        public int Id { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public decimal Gia { get; set; }

        public string MoTa { get; set; } = string.Empty;

        public int SoLuong { get; set; } 

        public IFormFile HinhAnh { get; set; } 
        public string? HinhAnh64 { get; set; }
    }
}
