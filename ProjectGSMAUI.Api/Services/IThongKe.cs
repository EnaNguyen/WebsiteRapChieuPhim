using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Services
{
    public interface IThongKe
    {
        Task<Dictionary<string, int>> GetRevenueByDate(string type);
        Task<Dictionary<string, int>> GetRevenueByMovie();
        //Task<Dictionary<string, int>> GetTicketSalesByDate(string type);
        Task<Dictionary<string, int>> GetTicketSalesByMovie();
    }
}
