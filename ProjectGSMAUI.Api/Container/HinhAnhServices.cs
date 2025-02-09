using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Helper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Services;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace ProjectGSMAUI.Api.Container
{
    public class HinhAnhServices : IHinhAnhServices
    {
        public Task<APIResponse> Create(HinhAnh data)
        {
            throw new NotImplementedException();
        }

        public Task<List<HinhAnh>> GetAll(string name)
        {
            throw new NotImplementedException();
        }

        public Task<HinhAnh> GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse> Update(HinhAnh data, int id)
        {
            throw new NotImplementedException();
        }
    }
}
