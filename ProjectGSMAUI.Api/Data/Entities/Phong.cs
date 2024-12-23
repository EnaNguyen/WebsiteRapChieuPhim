using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class Phong
{
    public int Id { get; set; }

    public string? TenPhong { get; set; }

    public int? SoLuongGhe { get; set; }

    public int? LoaiPhong { get; set; }

    public int? TinhTrang { get; set; }

    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();
}
