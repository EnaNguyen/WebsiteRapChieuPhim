using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Services
{
    public interface ICheckOutServices
    {
        Task<APIResponse> CreateDatVe(List<DatVeCreator> data);
    }
}
