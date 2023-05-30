using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class CsatResponeDistributionRequest
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int idCompany { get; set; }
    }
}