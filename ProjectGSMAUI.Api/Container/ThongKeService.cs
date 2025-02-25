using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public class ThongKeService : IThongKe
    {
        private readonly ApplicationDbContext _context;

        public ThongKeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetRevenueByDate(string type)
        {
            var query = _context.HoaDons.AsNoTracking()
                .Where(h => h.TinhTrang == 1)
                .Select(h => new
                {
                    NgayXuatKey = type == "day" ? h.NgayXuat.ToString() :
                                  type == "month" ? h.NgayXuat.Value.Month + "/" + h.NgayXuat.Value.Year :
                                  type == "year" ? h.NgayXuat.Value.Year.ToString() :
                                  h.NgayXuat.ToString(),
                    h.TongTien
                })
                .GroupBy(h => h.NgayXuatKey)
                .Select(g => new { Key = g.Key, Revenue = g.Sum(h => h.TongTien ?? 0) });

            return await query.ToDictionaryAsync(k => k.Key, v => v.Revenue);
        }


        public async Task<Dictionary<string, int>> GetRevenueByMovie()
        {
            var query = from hd in _context.HoaDons.AsNoTracking()
                        join ct in _context.ChiTietHoaDons on hd.MaHoaDon equals ct.MaHoaDon
                        join ve in _context.Ves on ct.MaVe equals ve.MaVe
                        join phim in _context.Phims on ve.MaPhim equals phim.Id
                        where hd.TinhTrang == 1
                        group hd by phim.TenPhim into g
                        select new { Movie = g.Key, Revenue = g.Sum(hd => hd.TongTien ?? 0) };

            return await query.ToDictionaryAsync(k => k.Movie, v => v.Revenue);
        }

        //public async Task<Dictionary<string, int>> GetTicketSalesByDate(string type)
        //{
        //    var query = _context.ChiTietHoaDons.AsNoTracking()
        //        .Include(ct => ct.HoaDon)
        //        .Where(ct => ct.HoaDon.TinhTrang == 1)
        //        .GroupBy(ct => type switch
        //        {
        //            "day" => ct.HoaDon.NgayXuat.ToString(),
        //            "month" => ct.HoaDon.NgayXuat.Value.Month + "/" + ct.HoaDon.NgayXuat.Value.Year,
        //            "year" => ct.HoaDon.NgayXuat.Value.Year.ToString(),
        //            _ => ct.HoaDon.NgayXuat.ToString()
        //        })
        //        .Select(g => new { Key = g.Key, TicketCount = g.Count() });

        //    return await query.ToDictionaryAsync(k => k.Key, v => v.TicketCount);
        //}

        public async Task<Dictionary<string, int>> GetTicketSalesByMovie()
        {
            var query = from ct in _context.ChiTietHoaDons.AsNoTracking()
                        join ve in _context.Ves on ct.MaVe equals ve.MaVe
                        join phim in _context.Phims on ve.MaPhim equals phim.Id
                        join hd in _context.HoaDons on ct.MaHoaDon equals hd.MaHoaDon
                        where hd.TinhTrang == 1
                        group ct by phim.TenPhim into g
                        select new { Movie = g.Key, TicketCount = g.Count() };

            return await query.ToDictionaryAsync(k => k.Movie, v => v.TicketCount);
        }
    }
}



