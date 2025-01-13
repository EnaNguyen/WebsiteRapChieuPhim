using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Services;
using ProjectGSMAUI.Api.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VesController : ControllerBase
    {
        private readonly IVeService _veService;

        public VesController(IVeService veService)
        {
            _veService = veService;
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetVes()
        {
            var ves = await _veService.GetAllAsync();
            return Ok(new APIResponse
            {
                ResponseCode = 200,
                Result = "Ves retrieved successfully",
                Data = ves
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetVe(string id)
        {
            var ve = await _veService.GetByIdAsync(id);
            if (ve == null)
            {
                return NotFound(new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Ve not found",
                    ErrorMessage = "The requested Ve does not exist."
                });
            }
            return Ok(new APIResponse
            {
                ResponseCode = 200,
                Result = "Ve retrieved successfully",
                Data = ve
            });
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateVe(Ve ve)
        {
            await _veService.CreateAsync(ve);
            return CreatedAtAction(nameof(GetVe), new { id = ve.MaVe }, new APIResponse
            {
                ResponseCode = 201,
                Result = "Ve created successfully",
                Data = ve
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVe(string id, Ve ve)
        {
            if (id != ve.MaVe)
            {
                return BadRequest(new APIResponse
                {
                    ResponseCode = 400,
                    Result = "Ve ID mismatch",
                    ErrorMessage = "The ID provided does not match the Ve ID."
                });
            }

            await _veService.UpdateAsync(ve);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVe(string id)
        {
            var ve = await _veService.GetByIdAsync(id);
            if (ve == null)
            {
                return NotFound(new APIResponse
                {
                    ResponseCode = 404,
                    Result = "Ve not found",
                    ErrorMessage = "The requested Ve does not exist."
                });
            }

            await _veService.DeleteAsync(id);
            return NoContent();
        }
    }
}
