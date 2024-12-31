using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProjectGSMAUI.Api.Controllers.PBH
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TaiKhoan>> GetTaiKhoans()
        {
            return _context.TaiKhoans.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<TaiKhoan> GetTaiKhoan(string id)
        {
            var taiKhoan = _context.TaiKhoans.Find(id);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            return taiKhoan;
        }

        [HttpPost]
        public ActionResult<TaiKhoan> PostTaiKhoan(TaiKhoan taiKhoan)
        {
            taiKhoan.MatKhau = PasswordHasher.HashPassword(taiKhoan.MatKhau);
            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            return CreatedAtAction("GetTaiKhoan", new { id = taiKhoan.IdtaiKhoan }, taiKhoan);
        }

        [HttpPut("{id}")]
        public IActionResult PutTaiKhoan(string id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.IdtaiKhoan)
            {
                return BadRequest();
            }

            taiKhoan.MatKhau = PasswordHasher.HashPassword(taiKhoan.MatKhau);
            _context.Entry(taiKhoan).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TaiKhoans.Any(e => e.IdtaiKhoan == id))
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
        public IActionResult DeleteTaiKhoan(string id)
        {
            var taiKhoan = _context.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            _context.TaiKhoans.Remove(taiKhoan);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
