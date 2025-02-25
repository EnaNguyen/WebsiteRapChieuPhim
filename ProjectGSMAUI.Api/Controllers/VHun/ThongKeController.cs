using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Controllers.VHun
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKe _thongKeService;

        public ThongKeController(IThongKe thongKeService)
        {
            _thongKeService = thongKeService;
        }

        // API: Lấy doanh thu theo ngày/tháng/năm
        [HttpGet("revenue-by-date")]
        public async Task<IActionResult> GetRevenueByDate([FromQuery] string type)
        {
            var result = await _thongKeService.GetRevenueByDate(type);
            return Ok(result);
        }

        // API: Lấy doanh thu theo phim
        [HttpGet("revenue-by-movie")]
        public async Task<IActionResult> GetRevenueByMovie()
        {
            var result = await _thongKeService.GetRevenueByMovie();
            return Ok(result);
        }

        // API: Lấy số lượng vé bán theo ngày/tháng/năm
        //[HttpGet("ticket-sales-by-date")]
        //public async Task<IActionResult> GetTicketSalesByDate([FromQuery] string type)
        //{
        //    var result = await _thongKeService.GetTicketSalesByDate(type);
        //    return Ok(result);
        //}

        // API: Lấy số lượng vé bán theo phim
        [HttpGet("ticket-sales-by-movie")]
        public async Task<IActionResult> GetTicketSalesByMovie()
        {
            var result = await _thongKeService.GetTicketSalesByMovie();
            return Ok(result);
        }
    }
}
