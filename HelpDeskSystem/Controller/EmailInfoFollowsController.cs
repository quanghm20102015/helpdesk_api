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
using Interfaces.Model.EmailInfoFollow;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailInfoFollowsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public EmailInfoFollowsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/EmailInfoFollows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailInfoFollow>>> GetEmailInfoFollows()
        {
          if (_context.EmailInfoFollows == null)
          {
              return NotFound();
          }
            return await _context.EmailInfoFollows.ToListAsync();
        }

        // GET: api/EmailInfoFollows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmailInfoFollow>> GetEmailInfoFollow(int id)
        {
          if (_context.EmailInfoFollows == null)
          {
              return NotFound();
          }
            var emailInfoFollow = await _context.EmailInfoFollows.FindAsync(id);

            if (emailInfoFollow == null)
            {
                return NotFound();
            }

            return emailInfoFollow;
        }

        // PUT: api/EmailInfoFollows/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmailInfoFollow(int id, EmailInfoFollow emailInfoFollow)
        {
            if (id != emailInfoFollow.id)
            {
                return BadRequest();
            }

            _context.Entry(emailInfoFollow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoFollowExists(id))
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

        // POST: api/EmailInfoFollows
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<EmailInfoFollowResponse> PostEmailInfoFollow(EmailInfoFollowRequest request)
        {
            if (_context.EmailInfoFollows == null)
            {
                return new EmailInfoFollowResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            _context.EmailInfoFollows.RemoveRange(_context.EmailInfoFollows.Where(x => x.idEmailInfo == request.idEmailInfo));
            _context.SaveChanges();

            foreach (int idUser in request.listFollow)
            {
                EmailInfoFollow obj = new EmailInfoFollow();
                obj.idEmailInfo = request.idEmailInfo;
                obj.idUser = idUser;

                _context.EmailInfoFollows.Add(obj);
                await _context.SaveChangesAsync();
            }

            return new EmailInfoFollowResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // DELETE: api/EmailInfoFollows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailInfoFollow(int id)
        {
            if (_context.EmailInfoFollows == null)
            {
                return NotFound();
            }
            var emailInfoFollow = await _context.EmailInfoFollows.FindAsync(id);
            if (emailInfoFollow == null)
            {
                return NotFound();
            }

            _context.EmailInfoFollows.Remove(emailInfoFollow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmailInfoFollowExists(int id)
        {
            return (_context.EmailInfoFollows?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
