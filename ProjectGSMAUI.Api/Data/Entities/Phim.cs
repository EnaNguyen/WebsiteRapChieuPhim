using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class Phim
{
    public int Id { get; set; }

    public string? TenPhim { get; set; }

    public int? TheLoai { get; set; }

    public int? ThoiLuong { get; set; }

    public string? DaoDien { get; set; }

    public int? GioiHanDoTuoi { get; set; }

    public DateOnly? NgayKhoiChieu { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public int? SoSuatChieu { get; set; }

    public int? TrangThai { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<HinhAnh> HinhAnhs { get; set; } = new List<HinhAnh>();

    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();

    public virtual TheLoaiPhim? TheLoaiNavigation { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
