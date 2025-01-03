namespace ProjectGSMAUI.Api.Modal
{
	public class ActiveGiamGia
	{
		public int MaGiamGia { get; set; }

		public string? TenGiamGia { get; set; }

		public int? GiaTri { get; set; }

		public DateOnly? NgayBatDau { get; set; }

		public DateOnly? NgayKetThuc { get; set; }

		public int? SoLuongConLai { get; set; }
		public int? SoLuongDaDung { get; set; }
	}
}
