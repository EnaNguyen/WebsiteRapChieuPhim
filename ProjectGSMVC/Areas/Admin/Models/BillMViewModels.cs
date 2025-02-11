using Newtonsoft.Json;

namespace ProjectGSMVC.Areas.Admin.Models

{
    public class BillMViewModels
    {
        public int MaHoaDon { get; set; }

        public int? MaDatVe { get; set; }

        public string? MaKhachHang { get; set; }

        public int? TongTien { get; set; }

        public int? MaGiamGia { get; set; }

        public DateOnly? NgayXuat { get; set; }

        public string? TinhTrangDisplay { get; set; }

        // Thêm thuộc tính này để chứa danh sách chi tiết hóa đơn

        [JsonProperty("detailBills")] // Ánh xạ với thuộc tính "detailBills" trong JSON
        public List<DetailBillViewModel> DetailBillItems { get; set; }
    }
}
