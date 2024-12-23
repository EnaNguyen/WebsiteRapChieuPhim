using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class Coupon
{
    public int Id { get; set; }

    public string? MaNhap { get; set; }

    public int? MaGiamGia { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual GiamGia? MaGiamGiaNavigation { get; set; }
}
