using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGSMVC.Areas.Admin.Models
{
    public class TheLoaiPhimViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public string? TenTheLoai { get; set; }
    }
}
