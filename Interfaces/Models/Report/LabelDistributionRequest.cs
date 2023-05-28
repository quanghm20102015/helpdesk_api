using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class LabelDistributionRequest
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int idCompany { get; set; }
        public int idLabel { get; set; }
    }
}