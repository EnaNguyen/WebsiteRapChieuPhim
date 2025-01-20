using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.MVC.Models
{
    public class VeModel
    {
        public string MaVe { get; set; } = string.Empty;
        public int? MaLichChieu { get; set; }
        public int? MaPhong { get; set; }
        public int? MaPhim { get; set; }
        public int? TinhTrang { get; set; }
        public string? MaGhe { get; set; }
        public DateTime? ThoiGianTao { get; set; }
    }
}