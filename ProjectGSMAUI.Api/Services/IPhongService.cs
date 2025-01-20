using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Container
{
    public interface IPhongService
    {
        Task<IEnumerable<Phong>> GetPhongsAsync();
        Task<Phong> GetPhongAsync(int id);
        Task<Phong> CreatePhongAsync(Phong phong);
        Task UpdatePhongAsync(Phong phong);
        Task DeletePhongAsync(int id);
    }
}