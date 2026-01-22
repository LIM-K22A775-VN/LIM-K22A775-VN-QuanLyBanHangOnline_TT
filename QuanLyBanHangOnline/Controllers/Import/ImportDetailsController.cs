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
    public class ImportDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImportDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ImportDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportDetail>>> GetImportDetail()
        {
          if (_context.ImportDetail == null)
          {
              return NotFound();
          }
            return await _context.ImportDetail.ToListAsync();
        }

        // GET: api/ImportDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImportDetail>> GetImportDetail(int id)
        {
          if (_context.ImportDetail == null)
          {
              return NotFound();
          }
            var importDetail = await _context.ImportDetail.FindAsync(id);

            if (importDetail == null)
            {
                return NotFound();
            }

            return importDetail;
        }

        // PUT: api/ImportDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImportDetail(int id, ImportDetail importDetail)
        {
            if (id != importDetail.IdImportDetail)
            {
                return BadRequest();
            }

            _context.Entry(importDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImportDetailExists(id))
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

        // POST: api/ImportDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ImportDetail>> PostImportDetail(ImportDetail importDetail)
        {
          if (_context.ImportDetail == null)
          {
              return Problem("Entity set 'ImportDetailContext.ImportDetail'  is null.");
          }
            _context.ImportDetail.Add(importDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetImportDetail", new { id = importDetail.IdImportDetail }, importDetail);
        }

        // DELETE: api/ImportDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImportDetail(int id)
        {
            if (_context.ImportDetail == null)
            {
                return NotFound();
            }
            var importDetail = await _context.ImportDetail.FindAsync(id);
            if (importDetail == null)
            {
                return NotFound();
            }

            _context.ImportDetail.Remove(importDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ImportDetailExists(int id)
        {
            return (_context.ImportDetail?.Any(e => e.IdImportDetail == id)).GetValueOrDefault();
        }
    }
}
