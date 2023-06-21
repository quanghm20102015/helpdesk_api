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
    public class TeamsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public TeamsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
          if (_context.Teams == null)
          {
              return NotFound();
          }
            return await _context.Teams.ToListAsync();
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
          if (_context.Teams == null)
          {
              return NotFound();
          }
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutTeam(Team team)
        {
            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(team.id))
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

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
          if (_context.Teams == null)
          {
              return Problem("Entity set 'EF_DataContext.Teams'  is null.");
          }

            team.isDelete = false;
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeam", new { id = team.id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            team.isDelete = true;

            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamExists(int id)
        {
            return (_context.Teams?.Any(e => e.id == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<Team>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<Team> team = _context.Teams.Where(r => r.idCompany == idCompany).ToList();

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

    }
}
