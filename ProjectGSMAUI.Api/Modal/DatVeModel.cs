namespace ProjectGSMAUI.Api.Modal
{
    public class DatVeModel
    {
        public int MaPhim { get; set; }
        public DateOnly? NgayChieu { get; set; }
        public int CaChieu { get; set; }
        public List<string> Ghe { get; set; }
    }
}
