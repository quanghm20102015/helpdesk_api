using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class EmailInfoLabelRequest
    {
        public int id { get; set; }
        public int idEmailInfo { get; set; }
        public List<int> listLabel { get; set; }
    }
}