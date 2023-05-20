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
using Org.BouncyCastle.Asn1.Ocsp;
using static Interfaces.Model.EmailInfo.EmailInfoGetMenuCountResponse;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public LabelsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Labels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Label>>> GetLabels()
        {
          if (_context.Labels == null)
          {
              return NotFound();
          }
            return await _context.Labels.ToListAsync();
        }

        // GET: api/Labels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Label>> GetLabel(int id)
        {
          if (_context.Labels == null)
          {
              return NotFound();
          }
            var label = await _context.Labels.FindAsync(id);

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }


        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<Label>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<Label> label = _context.Labels.Where(r=>r.idCompany == idCompany).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        // PUT: api/Labels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<LabelResponse> PutLabel(Label label)
        {
            _context.Entry(label).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabelExists(label.id))
                {
                    return new LabelResponse
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

            return new LabelResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // POST: api/Labels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<LabelResponse> PostLabel(Label label)
        {
              if (_context.Labels == null)
                {
                return new LabelResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Entity set 'EF_DataContext.Labels'  is null."
                };
              }
            _context.Labels.Add(label);
            await _context.SaveChangesAsync();

            return new LabelResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // DELETE: api/Labels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabel(int id)
        {
            if (_context.Labels == null)
            {
                return NotFound();
            }
            var label = await _context.Labels.FindAsync(id);
            if (label == null)
            {
                return NotFound();
            }

            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LabelExists(int id)
        {
            return (_context.Labels?.Any(e => e.id == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Route("GetMenuByIdCompany")]
        public async Task<ActionResult<List<dynamic>>> GetMenuByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<Label> label = _context.Labels.Where(r => r.idCompany == idCompany).ToList();

            List<dynamic> result = new List<dynamic>();
            foreach(Label obj in label)
            {
                dynamic objLabel = new System.Dynamic.ExpandoObject();

                List<EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idLabel == obj.id).ToList(); 
                List<int> listIdEmailInfoLabel = new List<int>();

                foreach (EmailInfoLabel emailInfoLabel in listEmailInfoLabel)
                {
                    listIdEmailInfoLabel.Add(emailInfoLabel.idEmailInfo.Value);
                }
                int countEmailInfo = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.mainConversation == true && r.isDelete == false
                && listIdEmailInfoLabel.Contains(r.id)).Count();


                objLabel.id = obj.id;
                objLabel.name = obj.name;
                objLabel.description = obj.description;
                objLabel.idCompany = obj.idCompany;
                objLabel.emailInfoCount = countEmailInfo;
                result.Add(objLabel);
            }

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }
    }
}
