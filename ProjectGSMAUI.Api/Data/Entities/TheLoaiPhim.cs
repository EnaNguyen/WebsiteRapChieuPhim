using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class TheLoaiPhim
{
    public int Id { get; set; }

    public string? TenTheLoai { get; set; }

    public virtual ICollection<Phim> Phims { get; set; } = new List<Phim>();
}
