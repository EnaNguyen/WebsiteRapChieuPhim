using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Container
{
    public interface IPhimService
    {
        Task<List<PhimView>> GetAll(string name);
        Task<DetailMovie> GetByID(int id);
        Task<APIResponse> Create(CreateMovie data);
        Task<APIResponse> Remove(int id);
        Task<APIResponse> Update(int Id, CreateMovie data);
        Task<APIResponse> CheckSuatChieu(int Id, CheckDate Data);
    }
}