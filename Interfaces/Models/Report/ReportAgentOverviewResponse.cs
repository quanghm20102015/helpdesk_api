using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class ReportAgentOverviewResponse : BaseResponse<ResponseStatus>
    {
        public ReportAgentOverviewObject result { get; set; }
        //public ListPerformentMonitor reportData { get; set; }
        
        public class ReportAgentOverviewObject
        {
            public int Total { get; set; }
            public int Online { get; set; }
            public int Busy { get; set; }
            public int Offline { get; set; }
        }
    }
}