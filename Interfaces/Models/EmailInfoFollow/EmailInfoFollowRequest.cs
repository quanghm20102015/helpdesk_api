using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoFollow
{
    public class EmailInfoFollowRequest
    {
        public int id { get; set; }
        public int idEmailInfo { get; set; }
        public List<int> listFollow { get; set; }
    }
}