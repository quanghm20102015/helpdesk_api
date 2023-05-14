using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class EmailInfoDeleteRequest
    {
        public int idEmailInfo { get; set; }
        public int idUserDelete { get; set; }
    }
}