using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Services;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Controllers.Quy
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherServices service;
        public VoucherController(IVoucherServices service, IMapper mapper)
        {
            this.service = service;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await  this.service.GetAll();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(int Id)
        {
            var data = await this.service.GetByID(Id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ActiveVoucher _data)
        {
            var data = await this.service.Create(_data);
            return Ok(data);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(ActiveVoucher _data, int id)
        {
            var data = await this.service.Update(_data,id);
            return Ok(data);
        }
        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(int id)
        {
            var data = await this.service.Remove(id);
            return Ok(data);
        }
    }
}
