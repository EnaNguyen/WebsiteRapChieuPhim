using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Modal
{
    public class HinhAnhRequest
    {
        public string Id { get; set; } = null!;

        public int? Phim { get; set; }

        public string? ImageData { get; set; }

        public virtual Phim? PhimNavigation { get; set; }
    }
}
