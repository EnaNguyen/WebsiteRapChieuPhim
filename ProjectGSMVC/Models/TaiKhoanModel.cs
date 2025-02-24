namespace ProjectGSMVC.Models
{
    public class TaiKhoanModel
    {
        public string IdTaiKhoan { get; set; } // ID tài khoản
        public string TenTaiKhoan { get; set; } // Tên đăng nhập
        public string TenNguoiDung { get; set; } // Họ tên đầy đủ
        public string MatKhau { get; set; }  // ✅ Đảm bảo có trường này
        public string ReMatKhau { get; set; } // ✅ Đảm bảo có trường này
        public int TrangThai { get; set; } // Trạng thái tài khoản (1: Active, 0: Inactive)
        public byte[] Hinh { get; set; }  // Đổi từ string sang byte[]
        public string Cccd { get; set; } // Số CCCD
        public int VaiTro { get; set; } // Vai trò (2: Admin, 1: khách hàng, ...)
        public string Email { get; set; } // Email
        public string Sdt { get; set; } // Số điện thoại
        public DateTime? NgaySinh { get; set; } // Ngày sinh
        public string DiaChi { get; set; } // Địa chỉ
        public bool? GioiTinh { get; set; } // Giới tính (true: Nam, false: Nữ)
        public DateTime? NgayDangKy { get; set; } // Ngày đăng ký
        public int DiemTichLuy { get; set; } // Điểm tích lũy
        public string HinhBase64
        {
            get
            {
                return Hinh != null ? Convert.ToBase64String(Hinh) : null;
            }
        }

        // 🛠️ Thêm thuộc tính HinhFile để upload ảnh từ form
        public IFormFile HinhFile { get; set; }
    }
}
