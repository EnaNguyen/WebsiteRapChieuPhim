using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class ChiTietHoaDon
{
    public int MaChiTietHoaDon { get; set; }

    public int? MaGhe { get; set; }

    public int? MaHoaDon { get; set; }

    public int? Gia { get; set; }

    public virtual Ve? MaGheNavigation { get; set; }

    public virtual HoaDon? MaHoaDonNavigation { get; set; }
}
