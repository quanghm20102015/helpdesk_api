using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class ReportOverviewResponse : BaseResponse<ResponseStatus>
    {
        public ReportOverview reportOverview { get; set; }
        public class ReportOverview
        {
            public int Opened { get; set; }
            public int Unattended { get; set; }
            public int Unassigned { get; set; }
            public int Resolved { get; set; }
        }
        public class PerformentMonitor
        {
            public string key { get; set; }
            public int value { get; set; }
        }
    }
}