using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ProjectGSMAUI.Api.Modal;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhimController : ControllerBase
    {
        private readonly IPhimService _phimService;

        private readonly ILogger<PhimController> _logger;
        public PhimController(IPhimService phimService, ILogger<PhimController> logger)
        {
            _phimService = phimService;
            _logger = logger;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string Name = null)
        {
            var data = await this._phimService.GetAll(Name);
            return Ok(data);
        }
        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(int Id)
        {
            var data = await this._phimService.GetByID(Id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPost("CreateLichChieu")]
        public async Task<IActionResult> CheckSuatChieu(int Id, CheckDate Data)
        {
            var data = await this._phimService.CheckSuatChieu(Id,Data);
            return Ok(data);
        }
        [HttpPost("CreatePhim")]
        public async Task<IActionResult> Create(CreateMovie Data)
        {
            var data = await this._phimService.Create(Data);
            return Ok(data);
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(int Id, CreateMovie Data)
        {
            var data = await this._phimService.Update(Id, Data);
            return Ok(data);
        }
    }
}