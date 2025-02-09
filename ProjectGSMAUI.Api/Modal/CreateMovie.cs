namespace ProjectGSMAUI.Api.Modal
{
    public class CreateMovie
    {
        public CreatePhim PhimDatas { get; set; }
        public List<CreateHinhAnh> HinhAnhs { get; set; }
        public List<CreateVideo> Videos { get; set; }

    }
}
