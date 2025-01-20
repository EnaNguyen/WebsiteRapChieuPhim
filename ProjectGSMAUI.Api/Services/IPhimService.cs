using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Container
{
    public interface IPhimService
    {
        Task<IEnumerable<Phim>> GetPhimsAsync();
        Task<Phim> GetPhimByIdAsync(int id);
        Task CreatePhimAsync(Phim phim);
        Task UpdatePhimAsync(Phim phim);
        Task DeletePhimAsync(int id);

        // New method for fetching all the Loai Phim
        Task<IEnumerable<TheLoaiPhim>> GetTheLoaisAsync();
    }
}