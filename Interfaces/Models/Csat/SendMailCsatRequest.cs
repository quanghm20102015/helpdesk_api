using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class SendMailCsatRequest
    {
        public string link { get; set; }
        public string to { get; set; }
        public string idGuIdEmailInfo { get; set; }
        public int idCompany { get; set; }
    }
}