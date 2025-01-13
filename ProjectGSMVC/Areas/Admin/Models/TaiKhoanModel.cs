namespace ProjectGSMVC.Areas.Admin.Models
{
    public class TaiKhoanModel
    {
        public string? TenTaiKhoan { get; set; }
        public string? MatKhau { get; set; }
        public string? ReMatKhau { get; set; }
        public string? TenNguoiDung { get; set; }
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? Hinh { get; set; }
        public string? Cccd { get; set; }
        public string? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
