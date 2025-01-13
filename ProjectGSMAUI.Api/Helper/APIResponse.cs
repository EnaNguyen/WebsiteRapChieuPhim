using ProjectGSMAUI.Api.Data.Entities;

namespace ProjectGSMAUI.Api.Helper
{
    public class APIResponse
    {
        public int ResponseCode {  get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}
