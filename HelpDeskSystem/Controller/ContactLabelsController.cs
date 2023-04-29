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
    public class ContactLabelsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public ContactLabelsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/ContactLabels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactLabel>>> GetContactLabels()
        {
          if (_context.ContactLabels == null)
          {
              return NotFound();
          }
            return await _context.ContactLabels.ToListAsync();
        }

        // GET: api/ContactLabels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactLabel>> GetContactLabel(int id)
        {
          if (_context.ContactLabels == null)
          {
              return NotFound();
          }
            var contactLabel = await _context.ContactLabels.FindAsync(id);

            if (contactLabel == null)
            {
                return NotFound();
            }

            return contactLabel;
        }

        // PUT: api/ContactLabels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactLabel(int id, ContactLabel contactLabel)
        {
            if (id != contactLabel.id)
            {
                return BadRequest();
            }

            _context.Entry(contactLabel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactLabelExists(id))
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

        // POST: api/ContactLabels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactLabel>> PostContactLabel(ContactLabel contactLabel)
        {
          if (_context.ContactLabels == null)
          {
              return Problem("Entity set 'EF_DataContext.ContactLabels'  is null.");
          }
            _context.ContactLabels.Add(contactLabel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactLabel", new { id = contactLabel.id }, contactLabel);
        }

        // DELETE: api/ContactLabels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactLabel(int id)
        {
            if (_context.ContactLabels == null)
            {
                return NotFound();
            }
            var contactLabel = await _context.ContactLabels.FindAsync(id);
            if (contactLabel == null)
            {
                return NotFound();
            }

            _context.ContactLabels.Remove(contactLabel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactLabelExists(int id)
        {
            return (_context.ContactLabels?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
