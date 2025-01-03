using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectGSMAUI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhimController : ControllerBase
    {
        private readonly IPhimService _phimService;

        public PhimController(IPhimService phimService)
        {
            _phimService = phimService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Phim>>> GetAll()
        {
            var phims = await _phimService.GetAllAsync();
            return Ok(phims);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Phim>> GetById(int id)
        {
            var phim = await _phimService.GetByIdAsync(id);
            if (phim == null)
            {
                return NotFound();
            }
            return Ok(phim);
        }

        [HttpPost]
        public async Task<ActionResult<Phim>> Create(Phim phim)
        {
            await _phimService.CreateAsync(phim);
            return CreatedAtAction(nameof(GetById), new { id = phim.Id }, phim);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Phim phim)
        {
            if (id != phim.Id)
            {
                return BadRequest();
            }
            await _phimService.UpdateAsync(phim);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _phimService.DeleteAsync(id);
            return NoContent();
        }
    }
}
