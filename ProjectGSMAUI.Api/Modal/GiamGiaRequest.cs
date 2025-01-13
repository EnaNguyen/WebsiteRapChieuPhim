namespace ProjectGSMAUI.Api.Modal
{
    public class GiamGiaRequest
    {
        public string TenGiamGia { get; set; }
        public DateOnly NgayBatDau {  get; set; }
        public DateOnly NgayKetThuc {  get; set; }
        public string MoTa {  get; set; }
        public int GiaTri {  get; set; }

        public int SoLuong { get; set; }
        public string ImageFile { get; set; }
    }
}
