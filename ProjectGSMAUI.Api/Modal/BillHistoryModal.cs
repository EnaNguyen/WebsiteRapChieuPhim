namespace ProjectGSMAUI.Api.Modal
{
    public class BillHistoryModal
    {
        public string TenPhim { get; set; }   
        public int MaHoaDon { get; set; }     
        public DateTime GioDatPhim { get; set; }
        public DateOnly NgayTaoHoaDon { get; set; } 
        public string TenUser { get; set; }
        public List<string> MaGhe { get; set; }   
    }


}
