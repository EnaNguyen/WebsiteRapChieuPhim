using System.ComponentModel.DataAnnotations;

namespace ProjectGSMAUI.Api.Data.Entities
{
    public class RefreshToken
    {
        [Key]
        public string UserID { get; set; }
        public string TokenID { get; set; }

        public string refreshtoken { get; set; }
    }
}
