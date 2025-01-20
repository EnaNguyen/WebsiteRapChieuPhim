using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Services
{
    public interface IHinhAnhServices
    {
        Task<APIResponse> Create(HinhAnh data);
    }
}
