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
    public class TeamAgentsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public TeamAgentsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/TeamAgents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamAgent>>> GetTeamAgents()
        {
          if (_context.TeamAgents == null)
          {
              return NotFound();
          }
            return await _context.TeamAgents.ToListAsync();
        }

        // GET: api/TeamAgents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamAgent>> GetTeamAgent(int id)
        {
          if (_context.TeamAgents == null)
          {
              return NotFound();
          }
            var teamAgent = await _context.TeamAgents.FindAsync(id);

            if (teamAgent == null)
            {
                return NotFound();
            }

            return teamAgent;
        }

        // PUT: api/TeamAgents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamAgent(int id, TeamAgent teamAgent)
        {
            if (id != teamAgent.id)
            {
                return BadRequest();
            }

            _context.Entry(teamAgent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamAgentExists(id))
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

        // POST: api/TeamAgents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamAgent>> PostTeamAgent(TeamAgent teamAgent)
        {
          if (_context.TeamAgents == null)
          {
              return Problem("Entity set 'EF_DataContext.TeamAgents'  is null.");
          }
            _context.TeamAgents.Add(teamAgent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeamAgent", new { id = teamAgent.id }, teamAgent);
        }

        // DELETE: api/TeamAgents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamAgent(int id)
        {
            if (_context.TeamAgents == null)
            {
                return NotFound();
            }
            var teamAgent = await _context.TeamAgents.FindAsync(id);
            if (teamAgent == null)
            {
                return NotFound();
            }

            _context.TeamAgents.Remove(teamAgent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamAgentExists(int id)
        {
            return (_context.TeamAgents?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
