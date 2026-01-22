using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Imports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Import>>> GetImport()
        {
          if (_context.Import == null)
          {
              return NotFound();
          }
            return await _context.Import.ToListAsync();
        }

        // GET: api/Imports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Import>> GetImport(int id)
        {
          if (_context.Import == null)
          {
              return NotFound();
          }
            var import = await _context.Import.FindAsync(id);

            if (import == null)
            {
                return NotFound();
            }

            return import;
        }

        // PUT: api/Imports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImport(int id, Import import)
        {
            if (id != import.IdImport)
            {
                return BadRequest();
            }

            _context.Entry(import).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImportExists(id))
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

        // POST: api/Imports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Import>> PostImport(Import import)
        {
          if (_context.Import == null)
          {
              return Problem("Entity set 'ImportContext.Import'  is null.");
          }
            _context.Import.Add(import);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetImport", new { id = import.IdImport }, import);
        }

        // DELETE: api/Imports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImport(int id)
        {
            if (_context.Import == null)
            {
                return NotFound();
            }
            var import = await _context.Import.FindAsync(id);
            if (import == null)
            {
                return NotFound();
            }

            _context.Import.Remove(import);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ImportExists(int id)
        {
            return (_context.Import?.Any(e => e.IdImport == id)).GetValueOrDefault();
        }
    }
}
