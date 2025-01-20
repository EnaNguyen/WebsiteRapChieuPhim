using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProjectGSMAUI.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongService _phongService;
        private readonly ILogger<PhongController> _logger;
        public PhongController(IPhongService phongService, ILogger<PhongController> logger)
        {
            _phongService = phongService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phong>>> GetPhongs()
        {
            var phongs = await _phongService.GetPhongsAsync();
            return Ok(phongs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Phong>> GetPhong(int id)
        {
            var phong = await _phongService.GetPhongAsync(id);

            if (phong == null)
            {
                return NotFound();
            }

            return phong;
        }


        [HttpPost]
        public async Task<ActionResult<Phong>> CreatePhong([FromBody] Phong model)
        {
            if (ModelState.IsValid)
            {
                var phong = await _phongService.CreatePhongAsync(model);
                return CreatedAtAction(nameof(GetPhong), new { id = phong.Id }, phong);
            }
            return BadRequest(ModelState);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhong(int id, [FromBody] Phong model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            try
            {
                await _phongService.UpdatePhongAsync(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _phongService.GetPhongAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhong(int id)
        {
            var phong = await _phongService.GetPhongAsync(id);
            if (phong == null)
            {
                return NotFound();
            }
            await _phongService.DeletePhongAsync(id);
            return NoContent();
        }
    }
}