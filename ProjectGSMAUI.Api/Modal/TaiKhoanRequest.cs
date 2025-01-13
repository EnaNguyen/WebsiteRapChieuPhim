namespace ProjectGSMAUI.Api.Modal
{
    public class TaiKhoanRequest
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
        public bool? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
    }
}
