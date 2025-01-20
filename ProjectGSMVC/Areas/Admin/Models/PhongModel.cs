using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.MVC.Models
{
    public class PhongModel
    {
        public int Id { get; set; }

        public string? TenPhong { get; set; }

        public int? SoLuongGhe { get; set; }

        public int? LoaiPhong { get; set; }

        public int? TinhTrang { get; set; }
    }
}