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
using Interfaces.Base;
using Org.BouncyCastle.Utilities;
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
                objEmailInfo.typeChannel = Common.Email;
                objEmailInfo.channelName = configMail.yourName;
                objEmailInfo.newConversation = (emailInfo.type == 2 && emailInfo.from == null) ? true : false;
                if (contact != null)
                {
                    objEmailInfo.idContact = contact.id;
                    objEmailInfo.contactName = contact.fullname.Replace("\"", "");
                }

                _context.EmailInfos.Where(c => c.idReference == emailInfo.messageId).ToList().ForEach(c => c.read = true);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                objEmailInfo = emailInfo;
            }


            List <EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idEmailInfo == id).ToList();
            List<Label> listLabel = _context.Labels.Where(r => r.idCompany == emailInfo.idCompany).ToList();
            List<Account> listAccount = _context.Accounts.Where(r => r.idCompany == emailInfo.idCompany).ToList();
            List<EmailInfo> listEmailInfo = _context.EmailInfos.Where(x => x.idReference == emailInfo.messageId && x.textBody != null && x.textBody != "").OrderBy(y => y.date).ToList();
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

            List<dynamic> listAssignDetail = new List<dynamic>();
            foreach (Account obj in listAccount)
            {
                dynamic obj1 = new System.Dynamic.ExpandoObject();
                //AccountDetail obj1 = new AccountDetail();
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

                if (obj.avatar != null)
                {
                    byte[] imageByteArray = Convert.FromBase64String(obj.avatar);

                    System.IO.File.WriteAllBytes(obj.fileName, imageByteArray);
                    obj1.avatar = imageByteArray;
                }
                else
                {
                    obj1.avatar = null;
                }

                listAssignDetail.Add(obj1);
            }

            List<dynamic> listFollowDetail = new List<dynamic>();
            foreach (Account obj in listAccount)
            {
                dynamic obj1 = new System.Dynamic.ExpandoObject();
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

                if (obj.avatar != null)
                {
                    byte[] imageByteArray = Convert.FromBase64String(obj.avatar);

                    System.IO.File.WriteAllBytes(obj.fileName, imageByteArray);
                    obj1.avatar = imageByteArray;
                }
                else
                {
                    obj1.avatar = null;
                }

                listFollowDetail.Add(obj1);
            }

            List<dynamic> listEmailInfoDynamic = new List<dynamic>();
            foreach (EmailInfo obj in listEmailInfo)
            {
                try
                {
                    List<EmailInfoAttach> listEmailInfoAttach = _context.EmailInfoAttachs.Where(r => r.idEmailInfo == obj.id).ToList();

                    dynamic obj1 = new System.Dynamic.ExpandoObject();

                    obj1.EmailInfo = obj;
                    obj1.ListAttach = listEmailInfoAttach;

                    listEmailInfoDynamic.Add(obj1);
                }
                catch (Exception ex)
                {
                }
            }

            return new EmailInfoResponse
            {
                Status = ResponseStatus.Susscess,
                emailInfo = objEmailInfo,
                listLabel = listLabelDetail,
                listAccount = listAccount.ToList<Object>(),
                listEmailInfo = listEmailInfoDynamic,
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
        public async Task<SendMailResponse> SendMail([FromForm] SendMailResquest request)
        {
            try
            {
                List<ConfigMail> configMail = _context.ConfigMails.Where(r => r.id == request.idConfigEmail).ToList();

                if (configMail.Count > 0)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(configMail[0].yourName, configMail[0].email));
                    message.To.Add(new MailboxAddress("", request.to));
                    message.Subject = "Re: " + request.subject;
                    //message.Body = new TextPart("html") { Text = request.body };
                    message.InReplyTo = request.messageId;
                    message.References.Add(request.messageId);


                    var emailBody = new MimeKit.BodyBuilder
                    {
                        HtmlBody = request.body
                    };
                    var _uploadedFiles = Request.Form.Files;


                    foreach (IFormFile sou in _uploadedFiles)
                    {
                        string FileName = sou.FileName;

                        //sou.CopyToAsync()
                        if (sou.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                sou.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                emailBody.Attachments.Add(FileName, fileBytes);
                            }

                            //string fullPath = Path.Combine(newPath, FileName);
                            //string SavedPath = folderPath + "/" + FileName;
                            //string fileType = Path.GetExtension(FileName).Replace(".", "");
                            //using (var stream = new FileStream(fullPath, FileMode.Create))
                            //{
                            //    sou.CopyTo(stream);
                            //}
                        }
                    }

                    // If you find that MimeKit does not properly auto-detect the mime-type based on the
                    // filename, you can specify a mime-type like this:
                    //emailBody.Attachments.Add ("Receipt.pdf", bytes, ContentType.Parse (MediaTypeNames.Application.Pdf));

                    message.Body = emailBody.ToMessageBody();


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
                emailInfo.subject = "Re: " + request.subject;
                emailInfo.textBody = request.body;
                emailInfo.idCompany = request.idCompany;
                emailInfo.status = 0;
                emailInfo.assign = request.assign;
                emailInfo.idGuId = Guid.NewGuid().ToString();
                emailInfo.type = 2;
                emailInfo.read = true;
                emailInfo.idReference = request.messageId;

                emailInfo.idContact = _context.Accounts.Where(x => x.workemail == emailInfo.to).FirstOrDefault().id;

                _context.EmailInfos.Add(emailInfo);
                await _context.SaveChangesAsync();


                try
                {
                    string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                    string folderPath = "Upload/EmailInfo/" + emailInfo.id;
                    string newPath = Path.Combine(webRootPath, folderPath);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    var _uploadedFiles = Request.Form.Files;
                    foreach (IFormFile sou in _uploadedFiles)
                    {
                        string FileName = sou.FileName;

                        if (sou.Length > 0)
                        {
                            string fullPath = Path.Combine(newPath, FileName);
                            string SavedPath = folderPath + "/" + FileName;
                            string fileType = Path.GetExtension(FileName).Replace(".", "");
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                sou.CopyTo(stream);

                                EmailInfoAttach obj = new EmailInfoAttach();
                                obj.idEmailInfo = emailInfo.id;
                                obj.pathFile = SavedPath;
                                obj.fileName = FileName;
                                _context.EmailInfoAttachs.Add(obj);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
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


        public static MimeMessage Reply(MimeMessage message, MailboxAddress from, bool replyToAll)
        {
            var reply = new MimeMessage();

            reply.From.Add(from);

            // reply to the sender of the message
            if (message.ReplyTo.Count > 0)
            {
                reply.To.AddRange(message.ReplyTo);
            }
            else if (message.From.Count > 0)
            {
                reply.To.AddRange(message.From);
            }
            else if (message.Sender != null)
            {
                reply.To.Add(message.Sender);
            }

            if (replyToAll)
            {
                // include all of the other original recipients - TODO: remove ourselves from these lists
                reply.To.AddRange(message.To);
                reply.Cc.AddRange(message.Cc);
            }

            // set the reply subject
            if (!message.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                reply.Subject = "Re: " + message.Subject;
            else
                reply.Subject = message.Subject;

            // construct the In-Reply-To and References headers
            if (!string.IsNullOrEmpty(message.MessageId))
            {
                reply.InReplyTo = message.MessageId;
                foreach (var id in message.References)
                    reply.References.Add(id);
                reply.References.Add(message.MessageId);
            }

            // quote the original message text
            using (var quoted = new StringWriter())
            {
                var sender = message.Sender ?? message.From.Mailboxes.FirstOrDefault();

                quoted.WriteLine("On {0}, {1} wrote:", message.Date.ToString("f"), !string.IsNullOrEmpty(sender.Name) ? sender.Name : sender.Address);
                using (var reader = new StringReader(message.TextBody))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        quoted.Write("> ");
                        quoted.WriteLine(line);
                    }
                }

                reply.Body = new TextPart("plain")
                {
                    Text = quoted.ToString()
                };
            }

            return reply;
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
                EmailInfoInsert.fromName = emailInfo.fullName;
                EmailInfoInsert.to = "";
                EmailInfoInsert.cc = "";
                EmailInfoInsert.bcc = "";
                EmailInfoInsert.subject = "";
                EmailInfoInsert.textBody =  "[Conversation " + Status.statusName + "]";
                EmailInfoInsert.assign = 0;
                EmailInfoInsert.status = 0;
                EmailInfoInsert.idLabel = 0;
                EmailInfoInsert.idGuId = Guid.NewGuid().ToString();
                EmailInfoInsert.type = Common.ChangeStatus;
                EmailInfoInsert.mainConversation = false;

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
        public async Task<EmailInfoGetFillteResponse> GetFillter(EmailInfoGetFillterRequest request)
        {
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

            List<EmailInfo> label = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && ((r.isDelete == false && request.idUserTrash == 0) || request.idUserTrash != 0) && r.mainConversation == true
            //&& (r.assign == request.assign || request.assign == 0)
            && (r.status == request.status || request.status == 0)
            && (listIdEmailLabel.Contains(r.id) || request.idLabel == 0)
            && (listIdEmailAssign.Contains(r.id) || request.assign == 0)
            && (listIdEmailFollow.Contains(r.id) || request.idUserFollow == 0)
            && ((r.from == null ? "" : r.from).ToUpper().Contains(request.textSearch.ToUpper()) || request.textSearch == "\"\"" || request.textSearch == ""
            || r.subject.ToUpper().Contains(request.textSearch.ToUpper())
            || r.textBody.ToUpper().Contains(request.textSearch.ToUpper())
            || r.fromName.ToUpper().Contains(request.textSearch.ToUpper()))
            && (r.idConfigEmail == request.idConfigEmail || request.idConfigEmail == 0)
            && ((request.unAssign == true && r.isAssign == false) || request.unAssign == false)
            && ((r.isDelete == true && r.idUserDelete == request.idUserTrash) || request.idUserTrash == 0)
            && (r.date >= (request.fromDate == null ? request.fromDate : request.fromDate.Value.ToUniversalTime()) || request.fromDate == null)
            && (r.date <= (request.toDate == null ? request.toDate : request.toDate.Value.ToUniversalTime()) || request.toDate == null)).OrderByDescending(x => x.date).ToList();

            List<dynamic> listDynamic = new List<dynamic>();

            foreach (EmailInfo obj in label)
            {
                dynamic objEmailInfo = new System.Dynamic.ExpandoObject();
                try
                {
                    objEmailInfo.id = obj.id;
                    objEmailInfo.idConfigEmail = obj.idConfigEmail;
                    objEmailInfo.messageId = obj.messageId;
                    objEmailInfo.date = obj.date;
                    objEmailInfo.from = obj.from;
                    objEmailInfo.fromName = obj.fromName.Replace("\"", "");
                    objEmailInfo.to = obj.to;
                    objEmailInfo.cc = obj.cc;
                    objEmailInfo.bcc = obj.bcc;
                    objEmailInfo.subject = obj.subject;
                    objEmailInfo.textBody = obj.textBody;
                    objEmailInfo.status = obj.status;
                    objEmailInfo.assign = obj.assign;
                    objEmailInfo.idCompany = obj.idCompany;
                    objEmailInfo.idLabel = obj.idLabel;
                    objEmailInfo.idGuId = obj.idGuId;
                    objEmailInfo.type = obj.type;
                    objEmailInfo.typeChannel = Common.Email;
                    objEmailInfo.countUnread = _context.EmailInfos.Where(x => x.idReference == obj.messageId && !x.read && !x.isDelete).Count();
                }
                catch (Exception ex)
                {
                    objEmailInfo = obj;
                }

                listDynamic.Add(objEmailInfo);
            }

            if (listDynamic == null)
            {
                return new EmailInfoGetFillteResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            return new EmailInfoGetFillteResponse
            {
                Status = ResponseStatus.Susscess,
                listEmailInfo = listDynamic,
                total = listDynamic.Count
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

            int all = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false).Count();
            int unAssign = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isAssign == false && r.isDelete == false && r.mainConversation == true).Count();


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
            int mine = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
            && listIdEmailAssign.Contains(r.id)).Count();
            int follow = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
            && listIdEmailFollow.Contains(r.id)).Count();

            int resolved = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
            && r.status == 2).Count();

            int trash = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == true && r.idUserDelete == request.idUser && r.mainConversation == true).Count();

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

        [HttpDelete]
        [Route("Trash")]
        public async Task<EmailInfoTrashResponse> Trash(EmailInfoTrashRequest request)
        {
            try
            {
                List<EmailInfo> listEmailInfo = _context.EmailInfos.Where(r => r.isDelete == true && request.listIdEmailInfo.Contains(r.id)).ToList();
                foreach (EmailInfo obj in listEmailInfo)
                {
                    _context.EmailInfos.Remove(obj);
                }
                await _context.SaveChangesAsync();
                return new EmailInfoTrashResponse
                {
                    Status = ResponseStatus.Susscess
                };
            }
            catch (Exception ex)
            {
                return new EmailInfoTrashResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
        }


        [HttpDelete]
        [Route("TrashAll")]
        public async Task<EmailInfoTrashResponse> TrashAll(int idUserTrash)
        {
            try
            {
                List<EmailInfo> listEmailInfo = _context.EmailInfos.Where(r => r.isDelete == true && r.idUserDelete == idUserTrash).ToList();
                foreach (EmailInfo obj in listEmailInfo)
                {
                    _context.EmailInfos.Remove(obj);
                }
                return new EmailInfoTrashResponse
                {
                    Status = ResponseStatus.Susscess
                };
            }
            catch (Exception ex)
            {
                return new EmailInfoTrashResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
        }

        [HttpPost]
        [Route("PrivateNote")]
        public async Task<EmailInfoPrivateNoteResponse> PrivateNote(EmailInfoPrivateNoteRequest request)
        {
            var EmailInfo = await _context.EmailInfos.FindAsync(request.idEmailInfo);
            var EmailInfoInsert = EmailInfo;

            try
            {
                await _context.SaveChangesAsync();
                EmailInfoInsert.id = 0;
                EmailInfoInsert.date = DateTime.Now.ToUniversalTime();
                EmailInfoInsert.from = "";
                EmailInfoInsert.fromName = request.fullName;
                EmailInfoInsert.to = "";
                EmailInfoInsert.cc = "";
                EmailInfoInsert.bcc = "";
                EmailInfoInsert.subject = "";
                EmailInfoInsert.textBody = request.privateNote;
                EmailInfoInsert.assign = 0;
                EmailInfoInsert.status = 0;
                EmailInfoInsert.idLabel = 0;
                EmailInfoInsert.idGuId = Guid.NewGuid().ToString();
                EmailInfoInsert.type = Common.PrivateNote;
                EmailInfoInsert.mainConversation = false;

                _context.EmailInfos.Add(EmailInfoInsert);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailInfoExists(request.idEmailInfo))
                {
                    return new EmailInfoPrivateNoteResponse
                    {
                        Status = ResponseStatus.Fail
                    };
                }
                else
                {
                    throw;
                }
            }

            return new EmailInfoPrivateNoteResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        [HttpPost]
        [Route("NewConversation")]
        public async Task<NewConversationResponse> NewConversation(NewConversationRequest request)
        {
            if (_context.EmailInfos == null)
            {
                return new NewConversationResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            if (request.listAgent.Count != 0)
            {
                foreach(int idAgent in request.listAgent)
                {
                    Account account = _context.Accounts.Where(x => x.id == idAgent).FirstOrDefault();

                    EmailInfo obj = new EmailInfo();
                    obj.messageId = Guid.NewGuid().ToString();
                    obj.idConfigEmail = request.idConfigEmail;
                    obj.date = DateTime.Now.ToUniversalTime();
                    obj.fromName = account.workemail;
                    obj.fromName = account.fullname;
                    obj.to = request.email;
                    obj.status = 1;
                    obj.idCompany = request.idCompany;
                    obj.idGuId = obj.messageId;
                    obj.type = 2;
                    obj.isDelete = false;
                    obj.mainConversation = true;
                    obj.read = true;
                    obj.idReference = obj.messageId;
                    _context.EmailInfos.Add(obj);
                    await _context.SaveChangesAsync();

                    foreach (int i in request.listAssign)
                    {
                        EmailInfoAssign objEmailInfoAssign = new EmailInfoAssign();
                        objEmailInfoAssign.idEmailInfo = obj.id;
                        objEmailInfoAssign.idUser = i;

                        _context.EmailInfoAssigns.Add(objEmailInfoAssign);
                        await _context.SaveChangesAsync();
                    }

                    foreach (int i in request.listLabel)
                    {
                        EmailInfoLabel objEmailInfoLabel = new EmailInfoLabel();
                        objEmailInfoLabel.idEmailInfo = obj.id;
                        objEmailInfoLabel.idLabel = i;

                        _context.EmailInfoLabels.Add(objEmailInfoLabel);
                        await _context.SaveChangesAsync();
                    }

                    foreach (int i in request.listFollow)
                    {
                        EmailInfoFollow objEmailInfoFollow = new EmailInfoFollow();
                        objEmailInfoFollow.idEmailInfo = obj.id;
                        objEmailInfoFollow.idUser = i;

                        _context.EmailInfoFollows.Add(objEmailInfoFollow);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                EmailInfo obj = new EmailInfo();
                obj.messageId = Guid.NewGuid().ToString();
                obj.idConfigEmail = request.idConfigEmail;
                obj.date = DateTime.Now.ToUniversalTime();
                obj.fromName = request.email;
                obj.fromName = request.userName;
                obj.to = request.email;
                obj.status = 1;
                obj.idCompany = request.idCompany;
                obj.idGuId = obj.messageId;
                obj.type = 2;
                obj.isDelete = false;
                obj.mainConversation = true;
                obj.read = true;
                obj.idReference = obj.messageId;
                _context.EmailInfos.Add(obj);
                await _context.SaveChangesAsync();

                foreach (int i in request.listAssign)
                {
                    EmailInfoAssign objEmailInfoAssign = new EmailInfoAssign();
                    objEmailInfoAssign.idEmailInfo = obj.id;
                    objEmailInfoAssign.idUser = i;

                    _context.EmailInfoAssigns.Add(objEmailInfoAssign);
                    await _context.SaveChangesAsync();
                }

                foreach (int i in request.listLabel)
                {
                    EmailInfoLabel objEmailInfoLabel = new EmailInfoLabel();
                    objEmailInfoLabel.idEmailInfo = obj.id;
                    objEmailInfoLabel.idLabel = i;

                    _context.EmailInfoLabels.Add(objEmailInfoLabel);
                    await _context.SaveChangesAsync();
                }

                foreach (int i in request.listFollow)
                {
                    EmailInfoFollow objEmailInfoFollow = new EmailInfoFollow();
                    objEmailInfoFollow.idEmailInfo = obj.id;
                    objEmailInfoFollow.idUser = i;

                    _context.EmailInfoFollows.Add(objEmailInfoFollow);
                    await _context.SaveChangesAsync();
                }
            }


            return new NewConversationResponse
            {
                Status = ResponseStatus.Susscess
            };
        }


        [HttpPost]
        [Route("SendMailNewConversation")]
        public async Task<SendMailResponse> SendMailNewConversation(SendMailResquest request)
        {
            try
            {
                List<ConfigMail> configMail = _context.ConfigMails.Where(r => r.id == request.idConfigEmail).ToList();

                if (configMail.Count > 0)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(configMail[0].yourName, configMail[0].email));
                    message.To.Add(new MailboxAddress("", request.to));
                    message.Subject = "Re: " + request.subject;
                    //message.Body = new TextPart("html") { Text = request.body };
                    ////message.InReplyTo = message.MessageId;

                    var emailBody = new MimeKit.BodyBuilder
                    {
                        HtmlBody = request.body
                    };
                    var _uploadedFiles = Request.Form.Files;


                    foreach (IFormFile sou in _uploadedFiles)
                    {
                        string FileName = sou.FileName;

                        //sou.CopyToAsync()
                        if (sou.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                sou.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                emailBody.Attachments.Add(FileName, fileBytes);
                            }
                        }
                    }

                    message.Body = emailBody.ToMessageBody();

                    var smtp = new SmtpClient();
                    smtp.Connect(configMail[0].outgoing, configMail[0].outgoingPort.Value);
                    smtp.Authenticate(configMail[0].email, configMail[0].password);
                    smtp.Send(message);
                    smtp.Disconnect(true);


                    EmailInfo emailInfo = new EmailInfo();
                    emailInfo = _context.EmailInfos.Where(r => r.messageId == request.messageId).FirstOrDefault();

                    emailInfo.messageId = message.MessageId;
                    emailInfo.idConfigEmail = request.idConfigEmail;
                    emailInfo.date = DateTime.Now.ToUniversalTime();
                    emailInfo.from = configMail[0].email;
                    //emailInfo.fromName = configMail[0].yourName;
                    emailInfo.to = request.to;
                    emailInfo.cc = request.cc;
                    emailInfo.bcc = request.bcc;
                    emailInfo.subject = request.subject;
                    emailInfo.textBody = request.body;
                    emailInfo.idCompany = request.idCompany;
                    emailInfo.status = 1;
                    emailInfo.assign = request.assign;
                    emailInfo.idGuId = Guid.NewGuid().ToString();
                    emailInfo.type = 2;
                    emailInfo.read = true;
                    emailInfo.idReference = emailInfo.messageId;
                    _context.Entry(emailInfo).State = EntityState.Modified;
                    await _context.SaveChangesAsync();


                    try
                    {
                        string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                        string folderPath = "Upload/EmailInfo/" + emailInfo.id;
                        string newPath = Path.Combine(webRootPath, folderPath);
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }

                        foreach (IFormFile sou in _uploadedFiles)
                        {
                            string FileName = sou.FileName;

                            if (sou.Length > 0)
                            {
                                string fullPath = Path.Combine(newPath, FileName);
                                string SavedPath = folderPath + "/" + FileName;
                                string fileType = Path.GetExtension(FileName).Replace(".", "");
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    sou.CopyTo(stream);

                                    EmailInfoAttach obj = new EmailInfoAttach();
                                    obj.idEmailInfo = emailInfo.id;
                                    obj.pathFile = SavedPath;
                                    obj.fileName = FileName;
                                    _context.EmailInfoAttachs.Add(obj);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
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
        
        [HttpPost]
        [Route("EmailInfoDownloadFile")]
        public async Task<IActionResult> EmailInfoDownloadFile(EmailInfoDownloadFileRequest request)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string pathFile = Path.Combine(webRootPath, request.pathFile);

            if (System.IO.File.Exists(pathFile))
            {
                return File(System.IO.File.OpenRead(pathFile), "application/octet-stream", Path.GetFileName(pathFile));
            }
            return NotFound();
        }
    }
}
