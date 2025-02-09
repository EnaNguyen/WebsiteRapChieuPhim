using ProjectGSMAUI.Api.Data.Entities;
namespace ProjectGSMAUI.Api.Modal
{
    public class DetailMovie
    {
        public PhimView  Movies { get;set; }
        public List<HinhAnh> HinhAnhs {get; set; }
        public List<Video> Videos { get; set; }
    }
}
