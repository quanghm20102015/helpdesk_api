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
    public class EmailInfoesController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public EmailInfoesController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/EmailInfoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailInfo>>> GetEmailInfos()
        {
          if (_context.EmailInfos == null)
          {
              return NotFound();
          }
            return await _context.EmailInfos.ToListAsync();
        }

        // GET: api/EmailInfoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmailInfo>> GetEmailInfo(int id)
        {
          if (_context.EmailInfos == null)
          {
              return NotFound();
          }
            var emailInfo = await _context.EmailInfos.FindAsync(id);

            if (emailInfo == null)
            {
                return NotFound();
            }

            return emailInfo;
        }

        // PUT: api/EmailInfoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmailInfo(int id, EmailInfo emailInfo)
        {
            if (id != emailInfo.id)
            {
                return BadRequest();
            }

            _context.Entry(emailInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoExists(id))
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

        // POST: api/EmailInfoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmailInfo>> PostEmailInfo(EmailInfo emailInfo)
        {
          if (_context.EmailInfos == null)
          {
              return Problem("Entity set 'EF_DataContext.EmailInfos'  is null.");
          }
            _context.EmailInfos.Add(emailInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmailInfo", new { id = emailInfo.id }, emailInfo);
        }

        // DELETE: api/EmailInfoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailInfo(int id)
        {
            if (_context.EmailInfos == null)
            {
                return NotFound();
            }
            var emailInfo = await _context.EmailInfos.FindAsync(id);
            if (emailInfo == null)
            {
                return NotFound();
            }

            _context.EmailInfos.Remove(emailInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmailInfoExists(int id)
        {
            return (_context.EmailInfos?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
