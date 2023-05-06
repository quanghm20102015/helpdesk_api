using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;
using Interfaces.Model.Account;
using Interfaces.Constants;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailInfoLabelsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public EmailInfoLabelsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/EmailInfoLabels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailInfoLabel>>> GetEmailInfoLabels()
        {
          if (_context.EmailInfoLabels == null)
          {
              return NotFound();
          }
            return await _context.EmailInfoLabels.ToListAsync();
        }

        // GET: api/EmailInfoLabels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmailInfoLabel>> GetEmailInfoLabel(int id)
        {
          if (_context.EmailInfoLabels == null)
          {
              return NotFound();
          }
            var emailInfoLabel = await _context.EmailInfoLabels.FindAsync(id);

            if (emailInfoLabel == null)
            {
                return NotFound();
            }

            return emailInfoLabel;
        }

        // PUT: api/EmailInfoLabels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmailInfoLabel(int id, EmailInfoLabel emailInfoLabel)
        {
            if (id != emailInfoLabel.id)
            {
                return BadRequest();
            }

            _context.Entry(emailInfoLabel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoLabelExists(id))
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

        // POST: api/EmailInfoLabels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<EmailInfoLabel>> PostEmailInfoLabel(EmailInfoLabel emailInfoLabel)
        //{
        //  if (_context.EmailInfoLabels == null)
        //  {
        //      return Problem("Entity set 'EF_DataContext.EmailInfoLabels'  is null.");
        //  }
        //    _context.EmailInfoLabels.Add(emailInfoLabel);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetEmailInfoLabel", new { id = emailInfoLabel.id }, emailInfoLabel);
        //}
        [HttpPost]
        public async Task<EmailInfoLabelResponse> PostEmailInfoLabel(EmailInfoLabelRequest request)
        {
            if (_context.EmailInfoLabels == null)
            {
                return new EmailInfoLabelResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            _context.EmailInfoLabels.RemoveRange(_context.EmailInfoLabels.Where(x => x.idEmailInfo == request.idEmailInfo));
            _context.SaveChanges();

            foreach (int idLabel in request.listLabel)
            {
                EmailInfoLabel obj = new EmailInfoLabel();
                obj.idEmailInfo = request.idEmailInfo;
                obj.idLabel = idLabel;

                _context.EmailInfoLabels.Add(obj);
                await _context.SaveChangesAsync();
            }

            return new EmailInfoLabelResponse
            {
                Status = ResponseStatus.Susscess
            };
        }
        // DELETE: api/EmailInfoLabels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailInfoLabel(int id)
        {
            if (_context.EmailInfoLabels == null)
            {
                return NotFound();
            }
            var emailInfoLabel = await _context.EmailInfoLabels.FindAsync(id);
            if (emailInfoLabel == null)
            {
                return NotFound();
            }

            _context.EmailInfoLabels.Remove(emailInfoLabel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmailInfoLabelExists(int id)
        {
            return (_context.EmailInfoLabels?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
