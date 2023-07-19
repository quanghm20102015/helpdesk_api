using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class ProfileRequest
    {
        public int id { get; set; }
        public string accountName { get; set; }
        public string displayName { get; set; }
    }
}