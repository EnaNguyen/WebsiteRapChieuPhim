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
        public IFormFile ImageFile { get; set; } // Đổi kiểu từ string thành IFormFile
        public string? ImageBase64 { get; set; } // Thuộc tính này để lưu hình ảnh dưới dạng Base64 nếu cần
    }
}
