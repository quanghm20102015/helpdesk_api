using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class SendMailResquest
    {
        public string to { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string? subject { get; set; }
        public string? body { get; set; }
        public int idCompany { get; set; }
        public int idConfigEmail { get; set; }
        public string messageId { get; set; }
        public int? assign { get; set; }
    }
}