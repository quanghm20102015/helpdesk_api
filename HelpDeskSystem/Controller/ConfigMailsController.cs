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
using MailKit.Net.Imap;
using System.Text;
using Interfaces.Model.ConfigMail;
using Interfaces.Base;

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
        public async Task<ConfigMailResponse> PutConfigMail(ConfigMail configMail)
        {
            _context.Entry(configMail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigMailExists(configMail.id))
                {
                    return new ConfigMailResponse
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

            return new ConfigMailResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // POST: api/ConfigMails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ConfigMailResponse> PostConfigMail(ConfigMail configMail)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(configMail.outgoing, configMail.outgoingPort.Value);
                    client.Authenticate(configMail.email, configMail.password);
                }
            }
            catch (Exception ex)
            {
                return new ConfigMailResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Outgoing or OutgoingPort incorrect"
                };
            }

            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect(configMail.incoming, configMail.incomingPort.Value, true);
                    client.Authenticate(configMail.email, configMail.password);
                }
            }
            catch (Exception ex)
            {
                return new ConfigMailResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Ingoing or IngoingPort incorrect"
                };
            }


            if (_context.ConfigMails == null)
            {
                return new ConfigMailResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Entity set 'EF_DataContext.Labels'  is null."
                };
            }
            _context.ConfigMails.Add(configMail);
            await _context.SaveChangesAsync();

            return new ConfigMailResponse
            {
                Status = ResponseStatus.Susscess
            };
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

        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ConfigMailByCompanyResponse> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return new ConfigMailByCompanyResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            List<ConfigMail> listConfigMail = _context.ConfigMails.Where(r => r.idCompany == idCompany).ToList();
            List<ConfigMailDetail> listConfigMailDetail = new List<ConfigMailDetail>();
            foreach (ConfigMail obj in listConfigMail)
            {
                ConfigMailDetail obj1 = new ConfigMailDetail();
                obj1.id = obj.id;
                obj1.email = obj.email;
                obj1.yourName = obj.yourName;
                obj1.idCompany = obj.idCompany;
                obj1.countEmail = 10;
                listConfigMailDetail.Add(obj1);
            }

            return new ConfigMailByCompanyResponse
            {
                Status = ResponseStatus.Susscess,
                listConfigMail = listConfigMailDetail
            };
        }

        [HttpGet]
        [Route("GetMenuByIdCompany")]
        public async Task<ActionResult<List<dynamic>>> GetMenuByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<ConfigMail> listConfigMail = _context.ConfigMails.Where(r => r.idCompany == idCompany).ToList();

            List<dynamic> result = new List<dynamic>();
            foreach (ConfigMail obj in listConfigMail)
            {
                dynamic objChannel = new System.Dynamic.ExpandoObject();

                int countEmailInfo = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.type == 1 && r.isDelete == false
                && r.idConfigEmail == obj.id).Count();


                objChannel.id = obj.id;
                objChannel.name = obj.yourName;
                objChannel.idCompany = obj.idCompany;
                objChannel.emailInfoCount = countEmailInfo;
                objChannel.typeChannel = Common.Email;
                result.Add(objChannel);
            }

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }
    }
}
