using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class ResetPasswordResquest
    {
        public string idGuId { get; set; }
        public string password { get; set; }
    }
}