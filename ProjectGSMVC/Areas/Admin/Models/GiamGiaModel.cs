namespace ProjectGSMVC.Areas.Admin.Models
{
    public class GiamGiaModel
    {
        public string TenGiamGia { get; set; }
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public string? MoTa { get; set; }
        public int GiaTri { get; set; }
        public int SoLuong { get; set; }
        public IFormFile ImageFile { get; set; } 
        public string? ImageBase64 { get; set; }
    }
}
