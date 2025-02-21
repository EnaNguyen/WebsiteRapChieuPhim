using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Container
{
    public interface IVeService
    {
        Task<IEnumerable<Ve>> GetVesAsync();
        Task<Ve> GetVeByIdAsync(string id);
        Task<IEnumerable<Ve>> GetFilteredVesAsync(string searching = null, DateTime? filterDate = null);
        Task CreateVeAsync(Ve ve);
        Task UpdateVeAsync(string id, Ve ve);
        Task DeleteVeAsync(string id);
        Task<List<string>> MuaVe(DatVeModel ve);
        Task<List<string>> GetVeByDate(int LichChieu);
    }
}