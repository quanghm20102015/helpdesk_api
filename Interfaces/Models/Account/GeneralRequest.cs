using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class GeneralRequest
    {
        public int id { get; set; }
        public string accountName { get; set; }
        public int? language { get; set; }
        public string companyName { get; set; }
        public int? numberDay { get; set; }
    }
}