using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsatsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public CsatsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Csats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Csat>>> GetCsats()
        {
          if (_context.Csats == null)
          {
              return NotFound();
          }
            return await _context.Csats.ToListAsync();
        }

        // GET: api/Csats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Csat>> GetCsat(int id)
        {
          if (_context.Csats == null)
          {
              return NotFound();
          }
            var csat = await _context.Csats.FindAsync(id);

            if (csat == null)
            {
                return NotFound();
            }

            return csat;
        }

        // PUT: api/Csats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCsat(int id, Csat csat)
        {
            if (id != csat.id)
            {
                return BadRequest();
            }

            _context.Entry(csat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CsatExists(id))
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

        // POST: api/Csats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Csat>> PostCsat(Csat csat)
        {
          if (_context.Csats == null)
          {
              return Problem("Entity set 'EF_DataContext.Csats'  is null.");
          }
            _context.Csats.Add(csat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCsat", new { id = csat.id }, csat);
        }

        // DELETE: api/Csats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCsat(int id)
        {
            if (_context.Csats == null)
            {
                return NotFound();
            }
            var csat = await _context.Csats.FindAsync(id);
            if (csat == null)
            {
                return NotFound();
            }

            _context.Csats.Remove(csat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CsatExists(int id)
        {
            return (_context.Csats?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
