using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using AutoMapper;
using ProjectGSMAUI.Api.Modal;
using ProjectGSMAUI.Api.Data.Entities;
using Azure.Core;

namespace ProjectGSMAUI.Api.Controllers.Quy
{
	[EnableRateLimiting("fixedwindow")]
	[Route("api/[controller]")]
	[ApiController]
	public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaServices service;
		private readonly IMapper mapper;
        public GiamGiaController(IGiamGiaServices service, IMapper mapper)
        {
            this.service = service;
			this.mapper = mapper;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string Name = null)
        {
            var data = await this.service.GetAll(Name);
            return Ok(data);
        }
        [HttpPost("Create")]
		public async Task<IActionResult> Create(GiamGiaRequest _data)
		{
			var data = await this.service.Create(_data);
			return Ok(data);
		}
		[HttpDelete("Remove")]
		public async Task<IActionResult> Remove(int id)
		{
			var data = await this.service.Remove(id);
			return Ok(data);
		}
		[HttpPut("Update")]
		public async Task<IActionResult> Update([FromBody] GiamGiaEdit request)
		{
            int id = request.Id;
            GiamGiaRequest data = request.GiamGiaRequest;

            // Gọi phương thức Update hiện tại
            var response = await this.service.Update(data, id);

            if (response.ResponseCode == 200)
            {
                return Ok(response.Result);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
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
	}
}
