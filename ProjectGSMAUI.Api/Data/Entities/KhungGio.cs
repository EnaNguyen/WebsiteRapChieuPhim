using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class KhungGio
{
    public int Id { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }
    [JsonIgnore]
    public virtual ICollection<LichChieu> LichChieus { get; set; } = new List<LichChieu>();
}
