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
    public class PhongService : IPhongService
    {
        private readonly ApplicationDbContext _context;

        public PhongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Phong>> GetPhongsAsync()
        {
            return await _context.Phongs.ToListAsync();
        }

        public async Task<Phong> GetPhongAsync(int id)
        {
            return await _context.Phongs.FindAsync(id);
        }

        public async Task<Phong> CreatePhongAsync(Phong phong)
        {
            _context.Phongs.Add(phong);
            await _context.SaveChangesAsync();
            return phong;
        }

        public async Task UpdatePhongAsync(Phong phong)
        {
            _context.Entry(phong).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task DeletePhongAsync(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
                await _context.SaveChangesAsync();
            }
        }
    }
}