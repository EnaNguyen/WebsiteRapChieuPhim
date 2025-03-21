﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ProjectGSMAUI.Api.Data.Entities;

public partial class GiamGia
{
	public int MaGiamGia { get; set; }

	public string? TenGiamGia { get; set; }

	public int? GiaTri { get; set; }
	public string MoTa { get; set; }
	public byte[]? HinhAnh { get; set; }
	public DateOnly? NgayBatDau { get; set; }

	public DateOnly? NgayKetThuc { get; set; }

	public int? SoLuong { get; set; }

	public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}

