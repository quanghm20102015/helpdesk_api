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
using Interfaces.Model.EmailInfo;
using static Interfaces.Model.EmailInfo.EmailInfoGetMenuCountResponse;
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

            dynamic objEmailInfo = new System.Dynamic.ExpandoObject();

            ConfigMail configMail = _context.ConfigMails.Where(x => x.id == emailInfo.idConfigEmail).FirstOrDefault();
            Contact contact = _context.Contacts.Where(x => x.email == emailInfo.from).FirstOrDefault();
            try
            {
                objEmailInfo.id = emailInfo.id;
                objEmailInfo.idConfigEmail = emailInfo.idConfigEmail;
                objEmailInfo.messageId = emailInfo.messageId;
                objEmailInfo.date = emailInfo.date;
                objEmailInfo.from = emailInfo.from;
                objEmailInfo.fromName = emailInfo.fromName.Replace("\"", "");
                objEmailInfo.to = emailInfo.to;
                objEmailInfo.cc = emailInfo.cc;
                objEmailInfo.bcc = emailInfo.bcc;
                objEmailInfo.subject = emailInfo.subject;
                objEmailInfo.textBody = emailInfo.textBody;
                objEmailInfo.status = emailInfo.status;
                objEmailInfo.assign = emailInfo.assign;
                objEmailInfo.idCompany = emailInfo.idCompany;
                objEmailInfo.idLabel = emailInfo.idLabel;
                objEmailInfo.idGuId = emailInfo.idGuId;
                objEmailInfo.type = emailInfo.type;
                objEmailInfo.channelName = configMail.yourName;
                if (contact != null)
                {
                    objEmailInfo.idContact = contact.id;
                    objEmailInfo.contactName = contact.fullname.Replace("\"", "");
                }
            }
            catch (Exception ex)
            {
                objEmailInfo = emailInfo;
            }


            List <EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idEmailInfo == id).ToList();
            List<Label> listLabel = _context.Labels.Where(r => r.idCompany == emailInfo.idCompany).ToList();
            List<Account> listAccount = _context.Accounts.Where(r => r.idCompany == emailInfo.idCompany).ToList();
            List<EmailInfo> listEmailInfo = _context.EmailInfos.Where(x => x.messageId == emailInfo.messageId).OrderBy(y => y.date).ToList();
            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idEmailInfo == id).ToList();
            List<EmailInfoFollow> listEmailInfoFollow = _context.EmailInfoFollows.Where(x => x.idEmailInfo == id).ToList();
            List<History> listHistory = _context.Historys.Where(x => x.type == 1 && x.idDetail == id).ToList();

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

            List<AccountDetail> listAssignDetail = new List<AccountDetail>();
            foreach (Account obj in listAccount)
            {
                AccountDetail obj1 = new AccountDetail();
                obj1.id = obj.id;
                obj1.fullname = obj.fullname;
                obj1.workemail = obj.workemail;
                obj1.idGuId = obj.idGuId;
                obj1.check = false;

                foreach (EmailInfoAssign objEmailInfoAssign in listEmailInfoAssign)
                {
                    if (obj.id == objEmailInfoAssign.idUser)
                    {
                        obj1.check = true;
                        break;
                    }
                }
                listAssignDetail.Add(obj1);
            }

            List<AccountDetail> listFollowDetail = new List<AccountDetail>();
            foreach (Account obj in listAccount)
            {
                AccountDetail obj1 = new AccountDetail();
                obj1.id = obj.id;
                obj1.fullname = obj.fullname;
                obj1.workemail = obj.workemail;
                obj1.idGuId = obj.idGuId;
                obj1.check = false;

                foreach (EmailInfoFollow objEmailInfoFollow in listEmailInfoFollow)
                {
                    if (obj.id == objEmailInfoFollow.idUser)
                    {
                        obj1.check = true;
                        break;
                    }
                }
                listFollowDetail.Add(obj1);
            }

            return new EmailInfoResponse
            {
                Status = ResponseStatus.Susscess,
                emailInfo = objEmailInfo,
                listLabel = listLabelDetail,
                listAccount = listAccount.ToList<Object>(),
                listEmailInfo = listEmailInfo.ToList<Object>(),
                listAssign = listAssignDetail.ToList<Object>(),
                listFollow = listFollowDetail.ToList<Object>(),
                listHistory = listHistory.ToList<Object>()
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
            emailInfo.isDelete = false;
            _context.EmailInfos.Add(emailInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmailInfo", new { id = emailInfo.id }, emailInfo);
        }

        // DELETE: api/EmailInfoes/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteEmailInfo(int id)
        //{
        //    if (_context.EmailInfos == null)
        //    {
        //        return NotFound();
        //    }
        //    var emailInfo = await _context.EmailInfos.FindAsync(id);
        //    if (emailInfo == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.EmailInfos.Remove(emailInfo);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpDelete]
        public async Task<EmailInfoDeleteResponse> DeleteEmailInfo(EmailInfoDeleteRequest request)
        {
            if (_context.EmailInfos == null)
            {
                return new EmailInfoDeleteResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            var emailInfo = await _context.EmailInfos.FindAsync(request.idEmailInfo);
            if (emailInfo == null)
            {
                return new EmailInfoDeleteResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            emailInfo.isDelete = true;
            emailInfo.idUserDelete = request.idUserDelete;
            emailInfo.dateDelete = DateTime.Now.ToUniversalTime();

            _context.Entry(emailInfo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new EmailInfoDeleteResponse
            {
                Status = ResponseStatus.Susscess
            };
        }


        [HttpPost]
        [Route("SendMail")]
        public async Task<SendMailResponse> SendMail(SendMailResquest request)
        {
            try
            {
                //List<ConfigMail> configMail = _context.ConfigMails.Where(r => r.idCompany == request.idCompany).ToList();
                List<ConfigMail> configMail = _context.ConfigMails.Where(r => r.id == request.idConfigEmail).ToList();

                if (configMail.Count > 0)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(configMail[0].yourName, configMail[0].email));
                    message.To.Add(new MailboxAddress("", request.to));
                    message.Subject = request.subject;
                    //message.Body = new TextPart(TextFormat.Plain) { Text = request.body };
                    message.Body = new TextPart("html") { Text = request.body };

                    var smtp = new SmtpClient();
                    smtp.Connect(configMail[0].outgoing, configMail[0].outgoingPort.Value);
                    smtp.Authenticate(configMail[0].email, configMail[0].password);
                    smtp.Send(message);
                    smtp.Disconnect(true);
                }

                EmailInfo emailInfo = new EmailInfo();

                emailInfo.idConfigEmail = request.idConfigEmail;
                emailInfo.messageId = request.messageId;
                emailInfo.date = DateTime.Now.ToUniversalTime();
                emailInfo.from = configMail[0].email;
                emailInfo.fromName = configMail[0].yourName;
                emailInfo.to = request.to;
                emailInfo.cc = request.cc;
                emailInfo.bcc = request.bcc;
                emailInfo.subject = request.subject;
                emailInfo.textBody = request.body;
                emailInfo.idCompany = request.idCompany;
                emailInfo.status = 0;
                emailInfo.assign = request.assign;
                emailInfo.idGuId = Guid.NewGuid().ToString();
                emailInfo.type = 2;
                _context.EmailInfos.Add(emailInfo);
                await _context.SaveChangesAsync();

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
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.isDelete == false).OrderByDescending(x => x.date).ToList();

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
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && (r.status == request.status || request.status == 0)).OrderByDescending(x => x.date).ToList();

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
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false
            && (r.assign == request.assign || request.assign == 0)
            && (r.status == request.status || request.status == 0)).OrderByDescending(x => x.date).ToList();

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
            var Status = await _context.Status.FindAsync(emailInfo.status);
            EmailInfo.status = emailInfo.status;
            var EmailInfoInsert = EmailInfo;

            _context.Entry(EmailInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                EmailInfoInsert.id = 0;
                EmailInfoInsert.date = DateTime.Now.ToUniversalTime();
                EmailInfoInsert.from = "";
                EmailInfoInsert.fromName = "";
                EmailInfoInsert.to = "";
                EmailInfoInsert.cc = "";
                EmailInfoInsert.bcc = "";
                EmailInfoInsert.subject = "";
                EmailInfoInsert.textBody = emailInfo.fullName + " change status to " + Status.statusName;
                EmailInfoInsert.assign = 0;
                EmailInfoInsert.status = 0;
                EmailInfoInsert.idLabel = 0;
                EmailInfoInsert.idGuId = Guid.NewGuid().ToString();
                EmailInfoInsert.type = 3;

                _context.EmailInfos.Add(EmailInfoInsert);
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

        [HttpPut]
        [Route("UpdateAssign")]
        public async Task<EmailInfoResponse> UpdateAssign(EmailInfoRequest emailInfo)
        {
            var EmailInfo = await _context.EmailInfos.FindAsync(emailInfo.id);
            EmailInfo.assign = emailInfo.assign;

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
            List<EmailInfo> listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false
            && (request.status == 0 || request.status == r.status)).OrderByDescending(x => x.date).ToList();

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
            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idConfigEmail == idConfigEmail && r.isDelete == false).OrderByDescending(x => x.date).ToList();

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
            int listAll = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false
            && (r.status == request.status || request.status == 0)
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && (r.idLabel == request.idLabel || request.idLabel == 0)).ToList().Count;
            int listByAgent = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false
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


        [HttpPost]
        [Route("GetFillter")]
        public async Task<ActionResult<List<EmailInfo>>> GetFillter(EmailInfoGetFillterRequest request)
        {
            if(request.textSearch == "")
            {

            }
            if (_context.Accounts == null)
            {
                return NotFound();
            }

            List<EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idLabel == request.idLabel).ToList();
            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idUser == request.assign).ToList();
            List<EmailInfoFollow> listEmailInfoFollow = _context.EmailInfoFollows.Where(x => x.idUser == request.idUserFollow).ToList();
            List<int> listIdEmailLabel = new List<int>();
            List<int> listIdEmailAssign = new List<int>();
            List<int> listIdEmailFollow = new List<int>();
            foreach (EmailInfoLabel emailInfoLabel in listEmailInfoLabel) 
            {
                listIdEmailLabel.Add(emailInfoLabel.idEmailInfo.Value);
            }
            foreach (EmailInfoAssign emailInfoAssign in listEmailInfoAssign)
            {
                listIdEmailAssign.Add(emailInfoAssign.idEmailInfo.Value);
            }
            foreach (EmailInfoFollow emailInfoFollow in listEmailInfoFollow)
            {
                listIdEmailFollow.Add(emailInfoFollow.idEmailInfo.Value);
            }

            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && ((r.isDelete == false && request.idUserTrash == 0) || request.idUserTrash != 0) && r.type == 1
            //&& (r.assign == request.assign || request.assign == 0)
            && (r.status == request.status || request.status == 0)
            && (listIdEmailLabel.Contains(r.id) || request.idLabel == 0)
            && (listIdEmailAssign.Contains(r.id) || request.assign == 0)
            && (listIdEmailFollow.Contains(r.id) || request.idUserFollow == 0)
            && ((r.from.Contains(request.textSearch) || request.textSearch == "\"\"" || request.textSearch == "") || (r.subject.Contains(request.textSearch) || request.textSearch == "\"\"" || request.textSearch == ""))
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && ((request.unAssign == true && r.isAssign == false) || request.unAssign == false)
            && ((r.isDelete == true && r.idUserDelete == request.idUserTrash) || request.idUserTrash == 0)).OrderByDescending(x => x.date).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpPost]
        [Route("GetFillterCount")]
        public async Task<EmailInfoGetFillteResponse> GetFillterCount(EmailInfoGetFillterRequest request)
        {
            if (_context.Accounts == null)
            {
                return new EmailInfoGetFillteResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            List<EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idLabel == request.idLabel).ToList();
            List<int> listIdEmailLabel = new List<int>();
            foreach (EmailInfoLabel emailInfoLabel in listEmailInfoLabel)
            {
                listIdEmailLabel.Add(emailInfoLabel.idEmailInfo.Value);
            }

            int listAll = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.type == 1 && r.isDelete == false
            && (r.status == request.status || request.status == 0)
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && ((r.from.Contains(request.textSearch) || request.textSearch == "\"\"" || request.textSearch == "") || (r.subject.Contains(request.textSearch) || request.textSearch == "\"\"" || request.textSearch == ""))
            && (listIdEmailLabel.Contains(r.id) || request.idLabel == 0)).ToList().Count;
            int listByAgent = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.type == 1 && r.isDelete == false
             && (listIdEmailLabel.Contains(r.id) || request.idLabel == 0)
            && (r.status == request.status || request.status == 0)
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && ((r.from.Contains(request.textSearch) || request.textSearch == "\"\"" || request.textSearch == "") || (r.subject.Contains(request.textSearch) || request.textSearch == "\"\"" || request.textSearch == ""))
            && (r.assign == request.assign || request.assign == 0)).ToList().Count;

            return new EmailInfoGetFillteResponse
            {
                Status = ResponseStatus.Susscess,
                All = listAll,
                ByAgent = listByAgent
            };
        }

        [HttpPost]
        [Route("GetMenuCount")]
        public async Task<EmailInfoGetMenuCountResponse> GetMenuCount(EmailInfoGetMenuCountRequest request)
        {
            if (_context.Accounts == null)
            {
                return new EmailInfoGetMenuCountResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            int all = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.type == 1 && r.isDelete == false).Count();
            int unAssign = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isAssign == false && r.isDelete == false && r.type == 1).Count();


            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idUser == request.idUser).ToList();
            List<EmailInfoFollow> listEmailInfoFollow = _context.EmailInfoFollows.Where(x => x.idUser == request.idUser).ToList();
            List<int> listIdEmailAssign = new List<int>();
            List<int> listIdEmailFollow = new List<int>();
            EmailInfoCount emailInfoCount = new EmailInfoCount();

            foreach (EmailInfoAssign emailInfoAssign in listEmailInfoAssign)
            {
                listIdEmailAssign.Add(emailInfoAssign.idEmailInfo.Value);
            }
            foreach (EmailInfoFollow emailInfoFollow in listEmailInfoFollow)
            {
                listIdEmailFollow.Add(emailInfoFollow.idEmailInfo.Value);
            }
            int mine = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.type == 1 && r.isDelete == false
            && listIdEmailAssign.Contains(r.id)).Count();
            int follow = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.type == 1 && r.isDelete == false
            && listIdEmailFollow.Contains(r.id)).Count();

            int resolved = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.type == 1 && r.isDelete == false
            && r.status == 2).Count();

            int trash = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == true && r.idUserDelete == request.idUser).Count();

            emailInfoCount.All = all;
            emailInfoCount.Mine = mine;
            emailInfoCount.Following = follow;
            emailInfoCount.Resolved = resolved;
            emailInfoCount.Trash = trash;
            emailInfoCount.Unassigned = unAssign;

            return new EmailInfoGetMenuCountResponse
            {
                Status = ResponseStatus.Susscess,
                emailInfoCount = emailInfoCount
            };
        }
    }
}
