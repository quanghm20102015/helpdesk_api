using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class EmailInfoPrivateNoteRequest
    {
        public int idEmailInfo { get; set; }
        public string fullName { get; set; }
        public string privateNote { get; set; }
    }
}