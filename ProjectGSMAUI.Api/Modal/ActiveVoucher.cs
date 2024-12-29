namespace ProjectGSMAUI.Api.Modal
{
    public class ActiveVoucher
    {
        public int Id { get; set; }

        public string? MaNhap { get; set; }

        public int? MaGiamGia { get; set; }

        public bool? TrangThai { get; set; }
        public string? TenTrangThai { get; set; }
    }
}
