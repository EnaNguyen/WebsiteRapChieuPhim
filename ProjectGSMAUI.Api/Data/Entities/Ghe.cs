using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class Ghe
{
    public string MaGhe { get; set; } = null!;

    public string? SoHang { get; set; }

    public int? SoCot { get; set; }

    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
}
