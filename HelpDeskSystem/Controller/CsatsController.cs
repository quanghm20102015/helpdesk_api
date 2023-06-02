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
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsatsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public CsatsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Csats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Csat>>> GetCsats()
        {
          if (_context.Csats == null)
          {
              return NotFound();
          }
            return await _context.Csats.ToListAsync();
        }

        // GET: api/Csats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Csat>> GetCsat(int id)
        {
          if (_context.Csats == null)
          {
              return NotFound();
          }
            var csat = await _context.Csats.FindAsync(id);

            if (csat == null)
            {
                return NotFound();
            }

            return csat;
        }

        // PUT: api/Csats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCsat(int id, Csat csat)
        {
            if (id != csat.id)
            {
                return BadRequest();
            }

            _context.Entry(csat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CsatExists(id))
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

        // POST: api/Csats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<CsatResponse> PostCsat(Csat csat)
        {
            if (_context.Csats == null)
            {
                return new CsatResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Entity set 'EF_DataContext.Csats'  is null."
                };
            }

            csat.dateTime = DateTime.Now.ToUniversalTime();
            _context.Csats.Add(csat);
            await _context.SaveChangesAsync();

            return new CsatResponse
            {
                Status = ResponseStatus.Susscess,
                Message = ""
            };
        }

        // DELETE: api/Csats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCsat(int id)
        {
            if (_context.Csats == null)
            {
                return NotFound();
            }
            var csat = await _context.Csats.FindAsync(id);
            if (csat == null)
            {
                return NotFound();
            }

            _context.Csats.Remove(csat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CsatExists(int id)
        {
            return (_context.Csats?.Any(e => e.id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Route("SendMail")]
        public async Task<LoginResponse> SendMail(SendMailCsatRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.workemail == request.to && u.idCompany == request.idCompany);

            if (account != null)
            {
                string Email = "";
                string YourName = "";
                string Password = "";
                string Incoming = "";
                int IncomingPort = 0;
                string Outgoing = "";
                int OutgoingPort = 0;
                int IdConfigEmail = 0;

                var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
                var config = configuration.Build();
                Email = config["MailSettings:Mail"];
                YourName = config["MailSettings:DisplayName"];
                Password = config["MailSettings:Password"];
                Incoming = config["MailSettings:Incoming"];
                IncomingPort = int.Parse(config["MailSettings:IncomingPort"]);
                Outgoing = config["MailSettings:Outgoing"];
                OutgoingPort = int.Parse(config["MailSettings:OutgoingPort"]);


                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(YourName, Email));
                message.To.Add(new MailboxAddress("", request.to));
                message.Subject = "Survey";
                message.Body = new TextPart(TextFormat.Plain)
                {
                    Text = "Hello, " + request.to + ",\r\n"
                    + "Below is the link to the customer service survey. Please rate the service \r\n"
                    + request.link + "\r\n"
                };

                var smtp = new SmtpClient();
                smtp.Connect(Outgoing, OutgoingPort);
                smtp.Authenticate(Email, Password);
                smtp.Send(message);
                smtp.Disconnect(true);

                Csat csat = new Csat();
                csat.idGuIdEmailInfo = request.idGuIdEmailInfo;
                csat.dateTime = DateTime.Now.ToUniversalTime();
                csat.idFeedBack = 0;
                csat.idCompany = request.idCompany;
                _context.Csats.Add(csat);
                await _context.SaveChangesAsync();

                return new LoginResponse
                {
                    Status = ResponseStatus.Susscess
                };
            }
            else
            {
                return new LoginResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Email does not exist"
                };
            }
        }
        
        [HttpPut]
        [Route("UpdateCsat")]
        public async Task<CsatResponse> UpdateCsat(Csat csat)
        {
            if (_context.Csats == null)
            {
                return new CsatResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Entity set 'EF_DataContext.Csats'  is null."
                };
            }

            Csat csatObject = _context.Csats.Where(r => r.idGuIdEmailInfo == csat.idGuIdEmailInfo && r.idFeedBack == 0).FirstOrDefault();
            csatObject.idFeedBack = csat.idFeedBack;
            csatObject.idCompany = csat.idCompany;
            csatObject.descriptionFeedBack = csat.descriptionFeedBack;
            csatObject.dateTime = DateTime.Now.ToUniversalTime();

            _context.Entry(csatObject).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new CsatResponse
            {
                Status = ResponseStatus.Susscess,
                Message = ""
            };
        }

    }
}
