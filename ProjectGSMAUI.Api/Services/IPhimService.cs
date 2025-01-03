using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Services
{
    public interface IPhimService
    {
        Task<IEnumerable<Phim>> GetAllAsync();
        Task<Phim> GetByIdAsync(int id);
        Task<Phim> CreateAsync(Phim phim);
        Task UpdateAsync(Phim phim);
        Task DeleteAsync(int id);
    }
}

