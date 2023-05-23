using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class NewConversationRequest
    {
        public int idConfigEmail { get; set; }
        public int idCompany { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public List<int> listAgent { get; set; }
        public List<int> listAssign { get; set; }
        public List<int> listFollow { get; set; }
        public List<int> listLabel { get; set; }
    }
}