using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class SendMailResetPasswordRequest
    {
        public string linkConfirm { get; set; }
        public string to { get; set; }
    }
}