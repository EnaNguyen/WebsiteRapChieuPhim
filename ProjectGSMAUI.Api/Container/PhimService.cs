using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;

namespace ProjectGSMAUI.Api.Container
{
    public class PhimService : IPhimService
    {
        private readonly ApplicationDbContext _context;

        public PhimService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Phim>> GetPhimsAsync()
        {
            return await _context.Phims
                .Include(p => p.HinhAnhs)
                .Include(p => p.TheLoaiNavigation)
              .AsNoTracking()
               .ToListAsync();
        }

        public async Task<Phim> GetPhimByIdAsync(int id)
        {
            return await _context.Phims
                .Include(p => p.HinhAnhs)
                .Include(p => p.TheLoaiNavigation)
                .FirstOrDefaultAsync(p => p.Id == id);
            //Tra ve 1 lít Phim
        }

        public async Task CreatePhimAsync(Phim phim)
        {

            if (phim.TheLoai != null)
            {
                var theLoai = await _context.TheLoaiPhims.FindAsync(phim.TheLoai);
                if (theLoai != null)
                {
                    phim.TheLoaiNavigation = theLoai;
                }
                else
                {
                    throw new KeyNotFoundException("TheLoai not found");
                }
            }


            if (phim.ImageFiles != null)
            {
                phim.HinhAnhs = new List<HinhAnh>();
                foreach (var file in phim.ImageFiles)
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        phim.HinhAnhs.Add(new HinhAnh { Id = Guid.NewGuid().ToString(), ImageData = memoryStream.ToArray() });

                    }

                }
            }
            _context.Phims.Add(phim);
            await _context.SaveChangesAsync();
        }


        public async Task UpdatePhimAsync(Phim phim)
        {
            var existingPhim = await _context.Phims
                .Include(p => p.HinhAnhs)
                .Include(p => p.TheLoaiNavigation)
                .FirstOrDefaultAsync(p => p.Id == phim.Id);

            if (existingPhim == null)
            {
                throw new KeyNotFoundException("Phim không tồn tại");
            }
            if (phim.TheLoai != null)
            {
                var theLoai = await _context.TheLoaiPhims.FindAsync(phim.TheLoai);
                if (theLoai != null)
                {
                    existingPhim.TheLoaiNavigation = theLoai;
                }
                else
                {
                    throw new KeyNotFoundException("TheLoai not found");
                }
            }

            existingPhim.TenPhim = phim.TenPhim;
            existingPhim.ThoiLuong = phim.ThoiLuong;
            existingPhim.DaoDien = phim.DaoDien;
            existingPhim.GioiHanDoTuoi = phim.GioiHanDoTuoi;
            existingPhim.NgayKhoiChieu = phim.NgayKhoiChieu;
            existingPhim.NgayKetThuc = phim.NgayKetThuc;
            existingPhim.SoXuatChieu = phim.SoXuatChieu;
            existingPhim.TrangThai = phim.TrangThai;
            existingPhim.MoTa = phim.MoTa;

            if (phim.ImageFiles != null && phim.ImageFiles.Any())
            {

                if (existingPhim.HinhAnhs != null)
                {
                    _context.RemoveRange(existingPhim.HinhAnhs);
                    existingPhim.HinhAnhs.Clear();
                }
                foreach (var file in phim.ImageFiles)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        existingPhim.HinhAnhs.Add(new HinhAnh { Id = Guid.NewGuid().ToString(), ImageData = memoryStream.ToArray() });

                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletePhimAsync(int id)
        {
            var phim = await _context.Phims
                 .Include(p => p.HinhAnhs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (phim != null)
            {
                _context.RemoveRange(phim.HinhAnhs);
                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();
            }

        }

        // New Method Implementation
        public async Task<IEnumerable<TheLoaiPhim>> GetTheLoaisAsync()
        {
            return await _context.TheLoaiPhims.AsNoTracking().ToListAsync();
        }
    }
}