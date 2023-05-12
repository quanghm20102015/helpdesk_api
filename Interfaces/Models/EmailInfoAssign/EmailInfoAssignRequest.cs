using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoAssign
{
    public class EmailInfoAssignRequest
    {
        public int id { get; set; }
        public int idEmailInfo { get; set; }
        public List<int> listAssign { get; set; }
    }
}