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
    public class ConfigMailsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public ConfigMailsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/ConfigMails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfigMail>>> GetConfigMails()
        {
          if (_context.ConfigMails == null)
          {
              return NotFound();
          }
            return await _context.ConfigMails.ToListAsync();
        }

        // GET: api/ConfigMails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConfigMail>> GetConfigMail(int id)
        {
          if (_context.ConfigMails == null)
          {
              return NotFound();
          }
            var configMail = await _context.ConfigMails.FindAsync(id);

            if (configMail == null)
            {
                return NotFound();
            }

            return configMail;
        }

        // PUT: api/ConfigMails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfigMail(int id, ConfigMail configMail)
        {
            if (id != configMail.id)
            {
                return BadRequest();
            }

            _context.Entry(configMail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigMailExists(id))
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

        // POST: api/ConfigMails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ConfigMail>> PostConfigMail(ConfigMail configMail)
        {
          if (_context.ConfigMails == null)
          {
              return Problem("Entity set 'EF_DataContext.ConfigMails'  is null.");
          }
            _context.ConfigMails.Add(configMail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConfigMail", new { id = configMail.id }, configMail);
        }

        // DELETE: api/ConfigMails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfigMail(int id)
        {
            if (_context.ConfigMails == null)
            {
                return NotFound();
            }
            var configMail = await _context.ConfigMails.FindAsync(id);
            if (configMail == null)
            {
                return NotFound();
            }

            _context.ConfigMails.Remove(configMail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConfigMailExists(int id)
        {
            return (_context.ConfigMails?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
