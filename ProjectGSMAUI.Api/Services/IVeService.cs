using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public interface IVeService
    {
        Task<IEnumerable<Ve>> GetAllAsync();
        Task<Ve> GetByIdAsync(string id);
        Task<Ve> CreateAsync(Ve ve);
        Task UpdateAsync(Ve ve);
        Task DeleteAsync(string id);
    }
}
