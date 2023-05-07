using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;
using Interfaces.Constants;
using Interfaces.Model.Account;
using Interfaces.Model.Contact;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public ContactsController(EF_DataContext context)
        {
            _context = context;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
          if (_context.Contacts == null)
          {
              return NotFound();
          }
            return await _context.Contacts.ToListAsync();
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ContactResponse> GetContact(int id)
        {
            if (_context.Contacts == null)
            {
                return new ContactResponse
                {
                    Status = ResponseStatus.Fail
                };
            }
            var contact = await _context.Contacts.FindAsync(id);
            List<ContactLabel> listContactLabel = _context.ContactLabels.Where(x => x.idContact == id).ToList();
            List<Label> listLabel = _context.Labels.Where(r => r.idCompany == contact.idCompany).ToList();

            List<LabelDetail> listLabelDetail = new List<LabelDetail>();
            foreach (Label obj in listLabel)
            {
                LabelDetail obj1 = new LabelDetail();
                obj1.id = obj.id;
                obj1.name = obj.name;
                obj1.description = obj.description;
                obj1.color = obj.color;
                obj1.check = false;

                foreach (ContactLabel objContactLabel in listContactLabel)
                {
                    if (obj.id == objContactLabel.idLabel)
                    {
                        obj1.check = true;
                        break;
                    }
                }
                listLabelDetail.Add(obj1);
            }

            if (contact == null)
            {
                return new ContactResponse
                {
                    Status = ResponseStatus.Fail
                };
            }

            return new ContactResponse
            {
                Status = ResponseStatus.Susscess,
                contact = contact,
                listLabel = listLabelDetail,
            };
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ContactResponse> PutContact(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contact.id))
                {
                    return new ContactResponse
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

            return new ContactResponse
            {
                Status = ResponseStatus.Susscess
            };
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ContactResponse> PostContact(Contact contact)
        {
            try
            {
                if (_context.Contacts == null)
                {
                    return new ContactResponse
                    {
                        Status = ResponseStatus.Fail,
                        Message = ""
                    };
                }
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                return new ContactResponse
                {
                    Status = ResponseStatus.Susscess
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ContactResponse
                {
                    Status = ResponseStatus.Fail,
                    Message = ""
                };
            }
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            if (_context.Contacts == null)
            {
                return NotFound();
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(int id)
        {
            return (_context.Contacts?.Any(e => e.id == id)).GetValueOrDefault();
        }


        [HttpGet]
        [Route("GetByIdCompany")]
        public async Task<ActionResult<List<Contact>>> GetByIdCompany(int idCompany)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<Contact> label = _context.Contacts.Where(r => r.idCompany == idCompany).ToList();

            if (label == null)
            {
                return NotFound();
            }

            return label;
        }


        [HttpGet]
        [Route("GetByIdLabel")]
        public async Task<ActionResult<List<Contact>>> GetByIdLabel([FromQuery] ContactGetByLabelRequest request)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            List<Contact> contact = _context.Contacts.Where(r => r.idLabel == request.idLabel && r.idCompany == request.idCompany).ToList();

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }
    }
}
