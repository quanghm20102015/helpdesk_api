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
    public class AccountsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public AccountsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<SignupResponse> PostAccount(Account account)
        {
            string message = "";
            var accountEmail = await _context.Accounts.FirstOrDefaultAsync
                (u => u.workemail.Equals(account.workemail));

            var accountCompany = await _context.Accounts.FirstOrDefaultAsync
                (u => u.company.Equals(account.company));

            if(accountEmail != null)
            {
                message += "Email already exist" + System.Environment.NewLine;
            }
            if (accountCompany != null)
            {
                message += "Company already exist" + System.Environment.NewLine;
            }

            if (_context.Accounts == null)
            {
                return new SignupResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = ""
                };
            }
            else
            {
                if (message != "")
                {
                    return new SignupResponse
                    {
                        Status = ResponseStatus.Fail,
                        Message = message
                    };
                }
                else
                {
                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();

                    return new SignupResponse
                    {
                        Status = ResponseStatus.Susscess
                    };
                }
            }
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<LoginResponse> CheckLogin([FromBody] LoginRequest user)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.workemail.Equals(user.workemail) && u.password.Equals(user.password));


            if (account == null)
            {
                return new LoginResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Invalid login credentials. Please try again."
                };
            }

            return new LoginResponse
            {
                Status = ResponseStatus.Susscess
            };
        }
    }
}
