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
using Interfaces.Base;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text;
using System.Security.Principal;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Metadata;
using MailKit.Security;
using System.IO.Compression;

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
        public async Task<ActionResult<dynamic>> GetAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            dynamic objAccount = new System.Dynamic.ExpandoObject();

            objAccount.id = account.id;
            objAccount.fullname = account.fullname;
            objAccount.company = account.company;
            objAccount.workemail = account.workemail;
            objAccount.password = account.password;
            objAccount.idCompany = account.idCompany;
            objAccount.confirm = account.confirm;
            objAccount.login = account.login;
            objAccount.status = account.status;
            objAccount.idGuId = account.idGuId;
            if (account.avatar != null)
            {
                byte[] imageByteArray = Convert.FromBase64String(account.avatar);

                System.IO.File.WriteAllBytes(account.fileName, imageByteArray);
                objAccount.avatar = imageByteArray;
            }
            else
            {
                objAccount.avatar = null;
            }

            if (objAccount == null)
            {
                return NotFound();
            }

            return objAccount;
        }

        // GET: api/Accounts/5
        [HttpGet]
        [Route("GetByEmail")]
        public async Task<ActionResult<Account>> GetAccountByEmail(string workemail)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.workemail.Equals(workemail) && u.isDelete == false);
            //var account = await _context.Accounts.FindAsync(workemail);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<BaseResponse<ResponseStatus>> PutAccount(Account account)
        {
            //try
            //{
            //    var _uploadedFiles = Request.Form.Files;
            //    foreach (IFormFile sou in _uploadedFiles)
            //    {
            //        string FileName = sou.FileName;
            //        //sou.CopyToAsync()
            //        if (sou.Length > 0)
            //        {
            //            using (var ms = new MemoryStream())
            //            {
            //                sou.CopyTo(ms);
            //                var fileBytes = ms.ToArray();
            //                string s = Convert.ToBase64String(fileBytes);
            //                account.avatar = s;
            //                account.fileName = FileName;
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //}


            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.id))
                {
                    return new BaseResponse<ResponseStatus>
                    {
                        Status = ResponseStatus.Fail,
                        Message = "NotFound"
                    };
                }
                else
                {
                    throw;
                }
            }

            return new BaseResponse<ResponseStatus>
            {
                Status = ResponseStatus.Susscess
            };
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
            if (accountCompany != null && account.idCompany == 0)
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
                    if(account.idCompany == 0)
                    {
                        Company cpn = new Company();
                        cpn.companyName = account.company;

                        _context.Companys.Add(cpn);
                        _context.SaveChanges();

                        account.idCompany = cpn.id;
                    }

                    account.idGuId = Guid.NewGuid().ToString();
                    account.isDelete = false;


                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();

                    return new SignupResponse
                    {
                        Status = ResponseStatus.Susscess,
                        Message = "Signup account susscess",
                        Id = account.id,
                        idGuId = account.idGuId
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

            account.isDelete = true;

            _context.Entry(account).State = EntityState.Modified;
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
            else
            {
                if (!account.confirm)
                {
                    return new LoginResponse
                    {
                        Status = ResponseStatus.Fail,
                        Message = "Account is not activated\r\n"
                    };
                }
            }

            return new LoginResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        [HttpPost]
        [Route("PostLogin")]
        public async Task<LoginResponse> PostLogin([FromBody] LoginLogoutRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.id == request.idUser);

            account.login = true;

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.id))
                {
                    return new LoginResponse
                    {
                        Status = ResponseStatus.Fail
                    };
                }
                else
                {
                    throw;
                }
            }

            return new LoginResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        [HttpPost]
        [Route("PostLogout")]
        public async Task<LoginResponse> PostLogout([FromBody] LoginLogoutRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.id == request.idUser);

            account.login = false;

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.id))
                {
                    return new LoginResponse
                    {
                        Status = ResponseStatus.Fail
                    };
                }
                else
                {
                    throw;
                }
            }

            return new LoginResponse
            {
                Status = ResponseStatus.Susscess
            };
        }


        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<dynamic>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<Account> account = _context.Accounts.Where(r => r.idCompany == idCompany && r.isDelete == false).ToList();

            List<dynamic> result = new List<dynamic>();
            foreach (Account acc in account)
            {
                dynamic objAccount = new System.Dynamic.ExpandoObject();

                objAccount.id = acc.id;
                objAccount.fullname = acc.fullname;
                objAccount.company = acc.company;
                objAccount.workemail = acc.workemail;
                objAccount.password = acc.password;
                objAccount.idCompany = acc.idCompany;
                objAccount.confirm = acc.confirm;
                objAccount.login = acc.login;
                objAccount.status = acc.status;
                objAccount.idGuId = acc.idGuId;
                objAccount.role = acc.role;

                if (acc.avatar!= null && acc.avatar!="")
                {
                    byte[] imageByteArray = Convert.FromBase64String(acc.avatar);

                    System.IO.File.WriteAllBytes(acc.fileName, imageByteArray);
                    objAccount.avatar = imageByteArray;
                }
                else
                {
                    objAccount.avatar = null;
                }
                result.Add(objAccount);
            }


            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        [Route("ChangeStatus")]
        public async Task<LoginResponse> ChangeStatus([FromBody] LoginLogoutRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.id == request.idUser);

            account.status = request.status;

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.id))
                {
                    return new LoginResponse
                    {
                        Status = ResponseStatus.Fail
                    };
                }
                else
                {
                    throw;
                }
            }

            return new LoginResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        [HttpPost]
        [Route("SendMailConfirm")]
        public async Task<SendMailResponse> SendMailConfirm(SendMailConfirmResquest request)
        {
            try
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
                message.Subject = "Confirmation Instructions";
                message.Body = new TextPart(TextFormat.Plain) { Text = "Welcome, " + request.fullName + ",\r\n"
                    + "You can confirm your account email through the link below:\r\n"
                    + request.linkConfirm
                };

                var smtp = new SmtpClient();
                smtp.Connect(Outgoing, OutgoingPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(Email, Password);
                smtp.Send(message);
                smtp.Disconnect(true);

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
        [Route("ConfirmSigup")]
        public async Task<ConfirmSignupResponse> ConfirmSigup([FromBody] ConfirmSignupRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.idGuId == request.idGuId);

            account.confirm = true;

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.id))
                {
                    return new ConfirmSignupResponse
                    {
                        Status = ResponseStatus.Fail
                    };
                }
                else
                {
                    throw;
                }
            }

            return new ConfirmSignupResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        [HttpPost]
        [Route("SendMailResetPassword")]
        public async Task<LoginResponse> SendMailResetPassword(SendMailResetPasswordRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.workemail == request.to);
             
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
                message.Subject = "Confirmation Instructions";
                message.Body = new TextPart(TextFormat.Plain)
                {
                    Text = "Hello, " + request.to + ",\r\n"
                    + "Someone has requested a link to change your password.You can do this through the link below.\r\n"
                    + request.linkConfirm + request.idUser + "\r\n"
                    + "If you didn't request this, please ignore this email.\r\n"
                    + "Your password won't change until you access the link above and create a new one.\r\n"
                };

                var smtp = new SmtpClient();
                smtp.Connect(Outgoing, OutgoingPort);
                smtp.Authenticate(Email, Password);
                smtp.Send(message);
                smtp.Disconnect(true);

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

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<ResetPasswordResponse> ResetPassword([FromBody] ResetPasswordResquest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync
                (u => u.idGuId == request.idGuId);

            if(account != null)
            {
                account.password = request.password;

                _context.Entry(account).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.id))
                    {
                        return new ResetPasswordResponse
                        {
                            Status = ResponseStatus.Fail
                        };
                    }
                    else
                    {
                        throw;
                    }
                }

                return new ResetPasswordResponse
                {
                    Status = ResponseStatus.Susscess
                };
            }
            else
            {
                return new ResetPasswordResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
        }

        [HttpPost]
        [Route("UploadImage")]
        public async Task<ActionResult> OnPostUploadAsync()
        {
            bool Results = false;
            try
            {
                var _uploadedFiles = Request.Form.Files;
                foreach (IFormFile sou in _uploadedFiles)
                {
                    string FileNam = sou.FileName;
                    //sou.CopyToAsync()
                    if (sou.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            sou.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            // act on the Base64 data
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Ok(Results);
        }
    }
}
