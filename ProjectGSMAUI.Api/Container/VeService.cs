using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;

namespace ProjectGSMAUI.Api.Services
{
    public class VeService : IVeService
    {
        private readonly ApplicationDbContext _context;

        public VeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ve>> GetAllAsync()
        {
            return await _context.Ves.ToListAsync();
        }

        public async Task<Ve> GetByIdAsync(string id)
        {
            return await _context.Ves.FindAsync(id);
        }

        public async Task<Ve> CreateAsync(Ve ve)
        {
            _context.Ves.Add(ve);
            await _context.SaveChangesAsync();
            return ve;
        }

        public async Task UpdateAsync(Ve ve)
        {
            _context.Ves.Update(ve);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var ve = await _context.Ves.FindAsync(id);
            if (ve != null)
            {
                _context.Ves.Remove(ve);
                await _context.SaveChangesAsync();
            }
        }
    }
}
