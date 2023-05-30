using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class CsatResponeDetailRequest
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int idCompany { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}