using Interfaces.Base;
using Interfaces.Constants;
using Interfaces.Model.Account;
using System.Collections.Generic;

namespace Interfaces.Model.Contact
{
    public class ContactResponse : BaseResponse<ResponseStatus>
    {
        public Object contact { get; set; }
        public List<LabelDetail> listLabel { get; set; }
        public List<Object> listNote { get; set; }
    }
}