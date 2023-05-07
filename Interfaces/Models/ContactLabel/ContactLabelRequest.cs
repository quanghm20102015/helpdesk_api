using System.Collections.Generic;

namespace Interfaces.Model.ContactLabel
{
    public class ContactLabelRequest
    {
        public int id { get; set; }
        public int idContact { get; set; }
        public List<int> listLabel { get; set; }
    }
}