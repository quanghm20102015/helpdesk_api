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
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
//using System.Net.Mail;

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
            return await _context.EmailInfos.OrderByDescending(x=>x.id).ToListAsync();
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

        [HttpPost]
        [Route("SendMail")]
        public async Task<SendMailResponse> SendMail(SendMailResquest request)
        {
            try
            {
                List<ConfigMail> configMail = _context.ConfigMails.Where(r => r.idCompany == request.idCompany).ToList();

                if (configMail.Count > 0)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(configMail[0].yourName, configMail[0].email));
                    message.To.Add(new MailboxAddress("", request.to));
                    message.Subject = request.subject;
                    message.Body = new TextPart(TextFormat.Plain) { Text = request.body };

                    var smtp = new SmtpClient();
                    smtp.Connect(configMail[0].outgoing, configMail[0].outgoingPort.Value);
                    smtp.Authenticate(configMail[0].email, configMail[0].password);
                    smtp.Send(message);
                    smtp.Disconnect(true);
                }

                return new SendMailResponse
                {
                    Status = ResponseStatus.Susscess
                };
            }
            catch (Exception ex)
            {
                return new SendMailResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = ex.Message
                };
            }
        }

        private bool EmailInfoExists(int id)
        {
            return (_context.EmailInfos?.Any(e => e.id == id)).GetValueOrDefault();
        }


        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<EmailInfo>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == idCompany).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpGet]
        [Route("GetByStatus")]
        public async Task<ActionResult<List<EmailInfo>>> GetByStatus([FromQuery] EmailInfoSearchRequest request)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && (r.status == request.status || request.status == 5)).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpPut]
        [Route("UpdateStatus")]
        public async Task<EmailInfoResponse> UpdateStatus(EmailInfoRequest emailInfo)
        {
            var EmailInfo = await _context.EmailInfos.FindAsync(emailInfo.id);
            EmailInfo.status = emailInfo.status;

            _context.Entry(EmailInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoExists(emailInfo.id))
                {
                    return new EmailInfoResponse
                    {
                        Status = ResponseStatus.Fail
                    };
                }
                else
                {
                    throw;
                }
            }

            return new EmailInfoResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        [HttpGet]
        [Route("GetByIdLabel")]
        public async Task<ActionResult<List<EmailInfo>>> GetByIdLabel(int idLabel)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idLabel == idLabel).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

    }
}
