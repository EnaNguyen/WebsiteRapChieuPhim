using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class HinhAnh
{
    public string Id { get; set; } = null!;

    public int? Phim { get; set; }

    public string? Link { get; set; }

    public virtual Phim? PhimNavigation { get; set; }
}
