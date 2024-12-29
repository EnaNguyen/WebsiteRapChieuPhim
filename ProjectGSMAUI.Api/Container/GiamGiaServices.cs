using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Services;

namespace ProjectGSMAUI.Api.Container
{
    public class GiamGiaServices: IGiamGiaServices
    {
        private readonly ApplicationDbContext context;
        public GiamGiaServices(ApplicationDbContext _context)
        {
            context = _context;
        }
        public List<GiamGia> GetAll()
        {
            return this.context.GiamGia.ToList();
        }
    }
}
