using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class Phim
{
    [Key]
    public int Id { get; set; }

    public string? TenPhim { get; set; }
    public string? TheLoai { get; set; }

    public int? ThoiLuong { get; set; }

    public string? DaoDien { get; set; }

    public int? GioiHanDoTuoi { get; set; }

    public DateOnly? NgayKhoiChieu { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public int? SoXuatChieu { get; set; }

    public int? TrangThai { get; set; }

    public string? MoTa { get; set; }

    public List<HinhAnh> HinhAnhs { get; set; } = new List<HinhAnh>(); // Navigation property

    [NotMapped] // Not mapped to a database column
    public List<IFormFile>? ImageFiles { get; set; }

    [ForeignKey("TheLoai")]
    public virtual TheLoaiPhim? TheLoaiNavigation { get; set; }


    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}