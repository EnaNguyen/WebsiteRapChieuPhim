using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public interface ICombo
    {
        Task<List<ComboModal>> GetAll();
        Task<ComboModal> GetById(int id);
        Task<APIResponse> Create(ComboModal comboModal);
        Task<APIResponse> Update(int id, ComboModal comboModal);
        Task<APIResponse> Delete(int id);
    }
}
