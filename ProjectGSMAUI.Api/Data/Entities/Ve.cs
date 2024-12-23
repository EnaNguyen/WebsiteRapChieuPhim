using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class Ve
{
    public int MaVe { get; set; }

    public int? MaLichChieu { get; set; }

    public int? MaPhong { get; set; }

    public int? MaPhim { get; set; }

    public int? TinhTrang { get; set; }

    public string? MaGhe { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual Ghe? MaGheNavigation { get; set; }

    public virtual LichChieu? MaLichChieuNavigation { get; set; }
}
