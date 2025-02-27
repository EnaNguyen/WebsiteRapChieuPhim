using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class HoaDon
{
    public int MaHoaDon { get; set; }

    public string? MaKhachHang { get; set; }

    public int? TongTien { get; set; }

    public int? MaGiamGia { get; set; }

    public DateOnly? NgayXuat { get; set; }

    public int? TinhTrang { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual ICollection<ChiTietHoaDon1> ChiTietHoaDons1 { get; set; } = new List<ChiTietHoaDon1>();

    public virtual ICollection<ChiTietHoaDon2> ChiTietHoaDons2 { get; set; } = new List<ChiTietHoaDon2>();

    public virtual Coupon? MaGiamGiaNavigation { get; set; }

    public virtual TaiKhoan? MaKhachHangNavigation { get; set; }
}
