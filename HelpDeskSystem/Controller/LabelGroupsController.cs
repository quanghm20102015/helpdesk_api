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
    public class LabelGroupsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public LabelGroupsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/LabelGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabelGroup>>> GetLabelGroups()
        {
          if (_context.LabelGroups == null)
          {
              return NotFound();
          }
            return await _context.LabelGroups.ToListAsync();
        }

        // GET: api/LabelGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LabelGroup>> GetLabelGroup(int id)
        {
          if (_context.LabelGroups == null)
          {
              return NotFound();
          }
            var labelGroup = await _context.LabelGroups.FindAsync(id);

            if (labelGroup == null)
            {
                return NotFound();
            }

            return labelGroup;
        }

        // PUT: api/LabelGroups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<LabelGroupResponse> PutLabelGroup(LabelGroup labelGroup)
        {
            LabelGroup obj = _context.LabelGroups.Where(r => r.id == labelGroup.id).FirstOrDefault();
            if(obj.name == labelGroup.name)
            {
                return new LabelGroupResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = "Group already exists"
                };
            }

            _context.Entry(labelGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new LabelGroupResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            return new LabelGroupResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // POST: api/LabelGroups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LabelGroup>> PostLabelGroup(LabelGroup labelGroup)
        {
          if (_context.LabelGroups == null)
          {
              return Problem("Entity set 'EF_DataContext.LabelGroups'  is null.");
          }
            _context.LabelGroups.Add(labelGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLabelGroup", new { id = labelGroup.id }, labelGroup);
        }

        // DELETE: api/LabelGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabelGroup(int id)
        {
            if (_context.LabelGroups == null)
            {
                return NotFound();
            }
            var labelGroup = await _context.LabelGroups.FindAsync(id);
            if (labelGroup == null)
            {
                return NotFound();
            }

            _context.LabelGroups.Remove(labelGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LabelGroupExists(int id)
        {
            return (_context.LabelGroups?.Any(e => e.id == id)).GetValueOrDefault();
        }


        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<LabelGroup>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<LabelGroup> label = _context.LabelGroups.Where(r => r.idCompany == idCompany).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }

        [HttpGet]
        [Route("GetMenuByIdCompany")]
        public async Task<ActionResult<List<dynamic>>> GetMenuByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<LabelGroup> label = _context.LabelGroups.Where(r => r.idCompany == idCompany).ToList();

            List<dynamic> result = new List<dynamic>();
            foreach (LabelGroup obj in label)
            {
                dynamic objLabel = new System.Dynamic.ExpandoObject();

                //List<EmailInfoLabel> listEmailInfoLabel = _context.EmailInfoLabels.Where(x => x.idLabel == obj.id).ToList();
                //List<int> listIdEmailInfoLabel = new List<int>();

                //foreach (EmailInfoLabel emailInfoLabel in listEmailInfoLabel)
                //{
                //    listIdEmailInfoLabel.Add(emailInfoLabel.idEmailInfo.Value);
                //}
                //int countEmailInfo = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.mainConversation == true && r.isDelete == false
                //&& listIdEmailInfoLabel.Contains(r.id)).Count();

                int count = _context.ContactLabels.Where(r => r.idLabel == obj.id).Count();

                objLabel.id = obj.id;
                objLabel.name = obj.name;
                objLabel.description = obj.description;
                objLabel.idCompany = obj.idCompany;
                objLabel.emailInfoCount = count;
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
