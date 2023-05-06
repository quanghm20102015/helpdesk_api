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
using System.Security.Principal;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.EntityFrameworkCore.Internal;
using System.Dynamic;
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
        public async Task<EmailInfoResponse> GetEmailInfo(int id)
        {
            if (_context.EmailInfos == null)
            {
                return new EmailInfoResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            var emailInfo = await _context.EmailInfos.FindAsync(id);
            List<EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idEmailInfo == id).ToList();
            List<Label> listLabel = _context.Labels.Where(r => r.idCompany == emailInfo.idCompany).ToList();

            List<LabelDetail> listLabelDetail = new List<LabelDetail>();
            foreach (Label obj in listLabel)
            {
                LabelDetail obj1 = new LabelDetail();
                obj1.id = obj.id;
                obj1.name = obj.name;
                obj1.description = obj.description;
                obj1.color = obj.color;
                obj1.check = false;

                foreach (EmailInfoLabel objEmailInfoLabel in listEmailInfoLabel)
                {
                    if(obj.id == objEmailInfoLabel.idLabel)
                    {
                        obj1.check = true;
                        break;
                    }
                }
                listLabelDetail.Add(obj1);
            }


            return new EmailInfoResponse
            {
                Status = ResponseStatus.Susscess,
                emailInfo = emailInfo,
                listLabel = listLabelDetail
            };
        }

        // PUT: api/EmailInfoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutEmailInfo(EmailInfo emailInfo)
        {
            _context.Entry(emailInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoExists(emailInfo.id))
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

            emailInfo.idGuId = Guid.NewGuid().ToString();
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
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && (r.status == request.status || request.status == 0)).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpGet]
        [Route("GetByAgent")]
        public async Task<ActionResult<List<EmailInfo>>> GetByAgent([FromQuery] EmailInfoGetByAgentRequest request)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany
            && (r.assign == request.assign || request.assign == 0)
            && (r.status == request.status || request.status == 0)).ToList();

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
        public async Task<ActionResult<List<EmailInfo>>> GetByIdLabel([FromQuery] EmailInfoGetByLabelRequest request)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idLabel == request.idLable).ToList();
            List<EmailInfo> listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany 
            && (request.status == 0 || request.status == r.status)).ToList();

            List<int> numbers = new List<int>();
            foreach(EmailInfoLabel obj in listEmailInfoLabel)
            {
                numbers.Add(obj.idEmailInfo.Value);
            }

            var filteredOrders = from order in listEmailInfo
                                 where numbers.Contains(order.id)
                                 select order;

            if (filteredOrders == null)
            {
                return NotFound();
            }

            return filteredOrders.ToList();
        }


        [HttpGet]
        [Route("GetByIdConfigEmail")]
        public async Task<ActionResult<List<EmailInfo>>> GetByIdConfigEmail(int idConfigEmail)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idConfigEmail == idConfigEmail).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpGet]
        [Route("GetCountByCompanyAgent")]
        public async Task<EmailInfoCountByCAResponse> GetCountByCompanyAgent([FromQuery] EmailInfoCountByCARequest request)
        {
            if (_context.Accounts == null)
            {
                return new EmailInfoCountByCAResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            int listAll = _context.EmailInfos.Where(r => r.idCompany == request.idCompany
            && (r.status == request.status || request.status == 0)
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && (r.idLabel == request.idLabel || request.idLabel == 0)).ToList().Count;
            int listByAgent = _context.EmailInfos.Where(r => r.idCompany == request.idCompany
             && (r.idLabel == request.idLabel || request.idLabel == 0)
            && (r.status == request.status || request.status == 0)
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && (r.assign == request.assign || request.assign == 0)).ToList().Count;


            return new EmailInfoCountByCAResponse
            {
                Status = ResponseStatus.Susscess,
                All = listAll,
                ByAgent = listByAgent
            };
        }

    }
}
