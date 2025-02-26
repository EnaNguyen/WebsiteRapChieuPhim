    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProjectGSMAUI.Api.Services;  
    using ProjectGSMAUI.Api.Modal; 
    using Microsoft.AspNetCore.Mvc;
    using ProjectGSMAUI.Api.Container;
    using ProjectGSMAUI.Api.Services;
    using Microsoft.AspNetCore.Http;
    using AutoMapper;
    using ProjectGSMAUI.Api.Modal;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.RateLimiting;
    using Microsoft.AspNetCore.Authorization;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    namespace ProjectGSMAUI.Api.Controllers.VHun
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        public class Bill_ManagementController : ControllerBase
        {
            private readonly IBillMServices Service;

            // Constructor để inject service vào controller
            public Bill_ManagementController(IBillMServices billService, IMapper mapper)
            {
                Service = billService;
            }

            // Lấy tất cả hóa đơn
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var result = await this.Service.GetAll();
                if (result == null || !result.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy hóa đơn" });
                }

                return Ok(result);
            }

            [HttpGet]
            public async Task<IActionResult> GetByID(int id)
            {
                var result = await Service.GetByID(id);
                if (result == null)
                {
                    return NotFound(new { Message = "Hóa đơn không tồn tại" });
                }
                return Ok(result);
            }

            // Tạo hóa đơn mới
            [HttpPost]
            public async Task<IActionResult> Create(Billmodal _data)
            {
                var data = await this.Service.Create(_data);
                return Ok(data);
            }

            [HttpPut]
            public async Task<IActionResult> Update(int id)
            {
                var response = await Service.Update(id);
                if (response.ResponseCode == 200)
                {
                    return Ok(response);
                }
                return NotFound(new { Message = "Hóa đơn không tồn tại" });
            }

        // Xóa hóa đơn
        [HttpDelete]
            public async Task<IActionResult> Remove(int id)
            {
                var response = await Service.Remove(id);
                if (response.ResponseCode == 200)
                {
                    return Ok(response);
                }
                return NotFound(new { Message = "Hóa đơn không tồn tại" });
            }
        [HttpGet]
        public async Task<IActionResult> GetDetailsByID(int id)
        {
            var result = await Service.GetDetailsByID(id);
            
            if (result == null)
            {
                return NotFound(new { Message = "Hóa đơn không tồn tại hoặc không có chi tiết" });
            }

            return Ok(result);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetBillsByCustomerId(string id)
        {
            var result = await Service.GetUserBillHistory(id);
            if (result == null || result.Count == 0)
            {
                return NotFound(new { Message = "Không tìm thấy hóa đơn nào cho khách hàng này" });
            }

            return Ok(result);
        }
    }
}
