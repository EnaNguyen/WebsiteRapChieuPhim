using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public class PhimService : IPhimService
    {
        private readonly ApplicationDbContext _context;

        public PhimService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Phim>> GetAllAsync()
        {
            return await _context.Phims.ToListAsync();
        }

        public async Task<Phim> GetByIdAsync(int id)
        {
            return await _context.Phims.FindAsync(id);
        }

        public async Task<Phim> CreateAsync(Phim phim)
        {
            _context.Phims.Add(phim);
            await _context.SaveChangesAsync();
            return phim;
        }

        public async Task UpdateAsync(Phim phim)
        {
            _context.Phims.Update(phim);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var phim = await _context.Phims.FindAsync(id);
            if (phim != null)
            {
                _context.Phims.Remove(phim);
                await _context.SaveChangesAsync();
            }
        }
    }
}
