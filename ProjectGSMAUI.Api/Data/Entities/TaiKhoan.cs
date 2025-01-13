using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ProjectGSMAUI.Api.Data.Entities
{
    public partial class TaiKhoan
    {
        public string IdtaiKhoan { get; set; } = null!;
        public string? TenTaiKhoan { get; set; }
        public string? MatKhau { get; set; }
        public string? TenNguoiDung { get; set; }
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public int? VaiTro { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public DateOnly? NgayDangKy { get; set; }
        public int? TrangThai { get; set; }
        public int? DiemTichLuy { get; set; }
        public byte[]? Hinh { get; set; }
        public string? Cccd { get; set; }
        public bool? GioiTinh { get; set; }
        public string? DiaChi { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
