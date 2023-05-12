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
using Interfaces.Model.ConfigMail;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriesController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public HistoriesController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Histories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<History>>> GetHistorys()
        {
          if (_context.Historys == null)
          {
              return NotFound();
          }
            return await _context.Historys.ToListAsync();
        }

        // GET: api/Histories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<History>> GetHistory(int id)
        {
          if (_context.Historys == null)
          {
              return NotFound();
          }
            var history = await _context.Historys.FindAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

        // PUT: api/Histories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistory(int id, History history)
        {
            if (id != history.id)
            {
                return BadRequest();
            }

            _context.Entry(history).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistoryExists(id))
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

        // POST: api/Histories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<HistoryResponse> PostHistory(History history)
        {
            if (_context.Historys == null)
            {
                return new HistoryResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            history.time = DateTime.Now.ToUniversalTime();
            _context.Historys.Add(history);
            await _context.SaveChangesAsync();

            return new HistoryResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // DELETE: api/Histories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            if (_context.Historys == null)
            {
                return NotFound();
            }
            var history = await _context.Historys.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            _context.Historys.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistoryExists(int id)
        {
            return (_context.Historys?.Any(e => e.id == id)).GetValueOrDefault();
        }


        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<History>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<History> history = _context.Historys.Where(r => r.idCompany == idCompany).ToList();

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

    }
}
