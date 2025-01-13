using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class DatVe
{
    public int? MaDatVe { get; set; }

    public string? MaKhachHang { get; set; }

    public string? MaVe { get; set; }

    public DateOnly? NgayDat { get; set; }

    public int? TrangThai { get; set; }

    public virtual TaiKhoan? MaKhachHangNavigation { get; set; }

    public virtual Ve? MaVeNavigation { get; set; }
}
