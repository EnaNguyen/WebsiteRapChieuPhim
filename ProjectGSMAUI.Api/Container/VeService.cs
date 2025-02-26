using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
namespace ProjectGSMAUI.Api.Container
{
    public class VeService : IVeService
    {
        private readonly ApplicationDbContext _context;

        public VeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ve>> GetVesAsync()
        {
            return await _context.Ves
                .Include(v => v.MaGheNavigation)
                .Include(v => v.MaLichChieuNavigation)
                .Include(v => v.MaPhimNavigation)
                 .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Ve>> GetFilteredVesAsync(string searching = null, DateTime? filterDate = null)
        {
            var query = _context.Ves
                  .Include(v => v.MaGheNavigation)
                  .Include(v => v.MaLichChieuNavigation)
                  .Include(v => v.MaPhimNavigation)
                  .AsNoTracking()
                  .AsQueryable();


            if (!string.IsNullOrEmpty(searching))
            {
                query = query.Where(v => v.MaVe.Contains(searching));
            }

            if (filterDate.HasValue)
            {
                DateTime filterDateStart = filterDate.Value.Date;
                DateTime filterDateEnd = filterDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(v => v.ThoiGianTao >= filterDateStart && v.ThoiGianTao <= filterDateEnd);
            }

            return await query.ToListAsync();
        }


        public async Task<Ve> GetVeByIdAsync(string id)
        {
            return await _context.Ves
                 .Include(v => v.MaGheNavigation)
                .Include(v => v.MaLichChieuNavigation)
                .Include(v => v.MaPhimNavigation)
                .FirstOrDefaultAsync(v => v.MaVe == id);
        }

        public async Task CreateVeAsync(Ve ve)
        {

            if (ve.MaLichChieu != null)
            {
                var lichChieu = await _context.LichChieus.FindAsync(ve.MaLichChieu);
                if (lichChieu != null)
                {
                    ve.MaLichChieuNavigation = lichChieu;
                }
                else
                {
                    throw new KeyNotFoundException("LichChieu not found");
                }
            }

            if (ve.MaGhe != null)
            {
                var ghe = await _context.Ghes.FindAsync(ve.MaGhe);
                if (ghe != null)
                {
                    ve.MaGheNavigation = ghe;
                }
                else
                {
                    throw new KeyNotFoundException("Ghe not found");
                }
            }

            if (ve.MaPhim != null)
            {
                var phim = await _context.Phims.FindAsync(ve.MaPhim);
                if (phim != null)
                {
                    ve.MaPhimNavigation = phim;
                }
                else
                {
                    throw new KeyNotFoundException("Phim not found");
                }
            }

            _context.Ves.Add(ve);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateVeAsync(string id, Ve ve)
        {
            var existingVe = await _context.Ves.Include(v => v.MaGheNavigation)
               .Include(v => v.MaLichChieuNavigation)
               .Include(v => v.MaPhimNavigation)
               .FirstOrDefaultAsync(v => v.MaVe == id);


            if (existingVe == null)
            {
                throw new KeyNotFoundException("Vé không tồn tại");
            }

            if (ve.MaLichChieu != null)
            {
                var lichChieu = await _context.LichChieus.FindAsync(ve.MaLichChieu);
                if (lichChieu != null)
                {
                    existingVe.MaLichChieuNavigation = lichChieu;
                }
                else
                {
                    throw new KeyNotFoundException("LichChieu not found");
                }
            }
            if (ve.MaGhe != null)
            {
                var ghe = await _context.Ghes.FindAsync(ve.MaGhe);
                if (ghe != null)
                {
                    existingVe.MaGheNavigation = ghe;
                }
                else
                {
                    throw new KeyNotFoundException("Ghe not found");
                }
            }

            if (ve.MaPhim != null)
            {
                var phim = await _context.Phims.FindAsync(ve.MaPhim);
                if (phim != null)
                {
                    existingVe.MaPhimNavigation = phim;
                }
                else
                {
                    throw new KeyNotFoundException("Phim not found");
                }
            }
            existingVe.TinhTrang = ve.TinhTrang;
            existingVe.ThoiGianTao = ve.ThoiGianTao;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVeAsync(string id)
        {
            var ve = await _context.Ves.FirstOrDefaultAsync(v => v.MaVe == id);
            if (ve != null)
            {
                _context.Ves.Remove(ve);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<string>> MuaVe(DatVeModel ve)
        {
            var LichChieu = await _context.LichChieus.Where(g=>(g.NgayChieu==ve.NgayChieu && g.GioChieu==ve.CaChieu && g.MaPhim==ve.MaPhim)).FirstOrDefaultAsync();
            var MaVe = await _context.Ves
            .Select(v => int.Parse(v.MaVe)) 
            .ToListAsync();
            int MaxMaVe = MaVe.Max()+1;

            List<string> ListMaVe = new List<string>();
            foreach (var item in ve.Ghe)
            {
                string[] parts = item.Split('_');
                int hang = int.Parse(parts[0]);
                int cot = int.Parse(parts[1]);
                if (cot > 10)
                {
                    cot -= 2;

                }
                else if (cot >5)
                {
                    cot -=1;
                }
                string Temp = "ABCDEFGHIJKL";
                char KiHieuCot = Temp[cot-1];
                Ve newve = new Ve
                {
                    MaVe = (MaxMaVe).ToString(),
                    MaLichChieu = LichChieu.MaLichChieu,
                    MaPhong = LichChieu.MaPhong,
                    MaPhim = ve.MaPhim,
                    TinhTrang = 1,
                    MaGhe =  $"{KiHieuCot}{hang}",
                    ThoiGianTao = DateTime.Now,
                };
                _context.Ves.Add(newve);
                ListMaVe.Add(MaxMaVe.ToString());
                await _context.SaveChangesAsync();
                MaxMaVe++;
            };
            return ListMaVe;
        }
        public async Task<List<string>> GetVeByDate(int LichChieu)
        {
            var ListVe = await _context.Ves
                                .Where(g => g.MaLichChieu == LichChieu)
                                .Select(g => g.MaGhe.Trim())
                                .ToListAsync();
            return ListVe;
        }
    }
}