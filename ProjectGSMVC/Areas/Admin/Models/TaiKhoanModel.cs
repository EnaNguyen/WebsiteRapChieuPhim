//using System.ComponentModel.DataAnnotations;

//namespace ProjectGSMVC.Areas.Admin.Models
//{
//    public class TaiKhoanModel
//    {
//        public string IdtaiKhoan { get; set; }

//        [Required(ErrorMessage = "Tên tài khoản không được để trống.")]
//        [StringLength(50, ErrorMessage = "Tên tài khoản không được vượt quá 50 ký tự.")]
//        public string TenTaiKhoan { get; set; }

//        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
//        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
//        public string MatKhau { get; set; }

//        [Required(ErrorMessage = "Email không được để trống.")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
//        public string Email { get; set; }

//        [Required(ErrorMessage = "Tên người dùng không được để trống.")]
//        public string TenNguoiDung { get; set; }

//        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
//        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
//        public string Sdt { get; set; }

//        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
//        public string DiaChi { get; set; }

//        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
//        public DateTime NgaySinh { get; set; }

//        public int VaiTro { get; set; }
//        public int TrangThai { get; set; } = 1; // Mặc định là hoạt động
//    }


//}
