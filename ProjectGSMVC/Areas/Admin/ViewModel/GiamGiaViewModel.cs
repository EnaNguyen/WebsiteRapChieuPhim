namespace ProjectGSMVC.Areas.Admin.ViewModel
{
	public class GiamGiaViewModel
	{
		public int MaGiamGia { get; set; }

		public string? TenGiamGia { get; set; }

		public int? GiaTri { get; set; }
		public string? MoTa { get; set; }
		public byte[]? HinhAnh { get; set; }
		public DateOnly? NgayBatDau { get; set; }

		public DateOnly? NgayKetThuc { get; set; }

		public int? SoLuongConLai { get; set; }
		public int? SoLuongDaDung { get; set; }
	}
}
