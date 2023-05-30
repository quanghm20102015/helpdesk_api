using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class PerformentMonitorAgentRequest
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int idCompany { get; set; }
        public int type { get; set; }
        public int IdUser { get; set; }
    }
}