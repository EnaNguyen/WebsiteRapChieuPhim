using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class LichChieu
{
    public int MaLichChieu { get; set; }

    public DateOnly? NgayChieu { get; set; }

    public int? GioChieu { get; set; }

    public int? MaPhim { get; set; }

    public int? MaPhong { get; set; }

    public decimal? GiaVe { get; set; }

    public bool? TinhTrang { get; set; }
    [JsonIgnore]
    public virtual KhungGio? GioChieuNavigation { get; set; }
    [JsonIgnore]
    public virtual Phim? MaPhimNavigation { get; set; }
    [JsonIgnore]
    public virtual Phong? MaPhongNavigation { get; set; }
    [JsonIgnore]
    public virtual ICollection<Ve> Ves { get; set; } = new List<Ve>();
}
