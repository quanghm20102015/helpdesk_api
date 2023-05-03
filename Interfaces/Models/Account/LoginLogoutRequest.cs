using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class LoginLogoutRequest
    {
        public int idUser { get; set; }
        public int status { get; set; }
        public bool login { get; set; }
    }
}