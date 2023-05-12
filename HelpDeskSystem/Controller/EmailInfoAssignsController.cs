using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;
using Interfaces.Constants;
using Interfaces.Model.EmailInfoLabel;
using Interfaces.Model.EmailInfoAssign;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailInfoAssignsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public EmailInfoAssignsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/EmailInfoAssigns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailInfoAssign>>> GetEmailInfoAssigns()
        {
          if (_context.EmailInfoAssigns == null)
          {
              return NotFound();
          }
            return await _context.EmailInfoAssigns.ToListAsync();
        }

        // GET: api/EmailInfoAssigns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmailInfoAssign>> GetEmailInfoAssign(int id)
        {
          if (_context.EmailInfoAssigns == null)
          {
              return NotFound();
          }
            var emailInfoAssign = await _context.EmailInfoAssigns.FindAsync(id);

            if (emailInfoAssign == null)
            {
                return NotFound();
            }

            return emailInfoAssign;
        }

        // PUT: api/EmailInfoAssigns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmailInfoAssign(int id, EmailInfoAssign emailInfoAssign)
        {
            if (id != emailInfoAssign.id)
            {
                return BadRequest();
            }

            _context.Entry(emailInfoAssign).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoAssignExists(id))
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

        // POST: api/EmailInfoAssigns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<EmailInfoAssignResponse> PostEmailInfoAssign(EmailInfoAssignRequest request)
        {
            if (_context.EmailInfoAssigns == null)
            {
                return new EmailInfoAssignResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            _context.EmailInfoAssigns.RemoveRange(_context.EmailInfoAssigns.Where(x => x.idEmailInfo == request.idEmailInfo));
            _context.SaveChanges();

            foreach (int idUser in request.listAssign)
            {
                EmailInfoAssign obj = new EmailInfoAssign();
                obj.idEmailInfo = request.idEmailInfo;
                obj.idUser = idUser;

                _context.EmailInfoAssigns.Add(obj);
                await _context.SaveChangesAsync();
            }

            return new EmailInfoAssignResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // DELETE: api/EmailInfoAssigns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailInfoAssign(int id)
        {
            if (_context.EmailInfoAssigns == null)
            {
                return NotFound();
            }
            var emailInfoAssign = await _context.EmailInfoAssigns.FindAsync(id);
            if (emailInfoAssign == null)
            {
                return NotFound();
            }

            _context.EmailInfoAssigns.Remove(emailInfoAssign);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmailInfoAssignExists(int id)
        {
            return (_context.EmailInfoAssigns?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
