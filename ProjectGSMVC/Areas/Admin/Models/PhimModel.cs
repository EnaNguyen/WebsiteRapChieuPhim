using Microsoft.AspNetCore.Http;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;

namespace ProjectGSMVC.Areas.Admin.Models
{
    public class PhimModel
    {
        public int Id { get; set; }
        public string TenPhim { get; set; }
        public string TheLoai { get; set; }
        public int ThoiLuong { get; set; }
        public string DaoDien { get; set; }
        public int GioiHanDoTuoi { get; set; }
        public DateOnly NgayKhoiChieu { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public int SoXuatChieu { get; set; }
        public int TrangThai { get; set; }
        public string? MoTa { get; set; }

        // List of IFormFile for multiple image upload
        public List<IFormFile> ImageFiles { get; set; }
        public List<string?> ImageFiles64 { get; set; } = new List<string>();
       

    }
}