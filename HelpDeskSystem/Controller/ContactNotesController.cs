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
    public class ContactNotesController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public ContactNotesController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/ContactNotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactNote>>> GetContactNotes()
        {
          if (_context.ContactNotes == null)
          {
              return NotFound();
          }
            return await _context.ContactNotes.ToListAsync();
        }

        // GET: api/ContactNotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactNote>> GetContactNote(int id)
        {
          if (_context.ContactNotes == null)
          {
              return NotFound();
          }
            var contactNote = await _context.ContactNotes.FindAsync(id);

            if (contactNote == null)
            {
                return NotFound();
            }

            return contactNote;
        }

        // PUT: api/ContactNotes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactNote(int id, ContactNote contactNote)
        {
            if (id != contactNote.id)
            {
                return BadRequest();
            }

            _context.Entry(contactNote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactNoteExists(id))
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

        // POST: api/ContactNotes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ContactNote>> PostContactNote(ContactNote contactNote)
        {
          if (_context.ContactNotes == null)
          {
              return Problem("Entity set 'EF_DataContext.ContactNotes'  is null.");
          }

            contactNote.timeNote = DateTime.Now.ToUniversalTime();
            _context.ContactNotes.Add(contactNote);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactNote", new { id = contactNote.id }, contactNote);
        }

        // DELETE: api/ContactNotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactNote(int id)
        {
            if (_context.ContactNotes == null)
            {
                return NotFound();
            }
            var contactNote = await _context.ContactNotes.FindAsync(id);
            if (contactNote == null)
            {
                return NotFound();
            }

            _context.ContactNotes.Remove(contactNote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactNoteExists(int id)
        {
            return (_context.ContactNotes?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
