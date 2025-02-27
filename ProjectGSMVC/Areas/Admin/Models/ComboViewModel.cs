using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProjectGSMAUI.MVC.Models;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMVC.Areas.Admin.Models
{
    public class ComboViewModel
    {
        public int Id { get; set; }

        public string TenCombo { get; set; }


        public decimal Gia { get; set; }

        public string MoTa { get; set; }
        public string HinhAnh { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? ImageBase64 { get; set; }

        // ✅ KHỞI TẠO DANH SÁCH ĐỂ TRÁNH NULL
        public List<ChiTietComboViewModel> ChiTietCombos { get; set; } = new();
        public List<SanPhamModel> SanPhamList { get; set; } = new();
    }


    public class ChiTietComboViewModel
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; }

        
        public int SoLuong { get; set; }
    }
}
