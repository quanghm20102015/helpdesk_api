using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class ReportOverviewResponse : BaseResponse<ResponseStatus>
    {
        public ReportOverview result { get; set; }
        //public ListPerformentMonitor reportData { get; set; }
        
        public class ReportOverview
        {
            public ReportOverviewObject Opened { get; set; }
            public ReportOverviewObject Unattended { get; set; }
            public ReportOverviewObject Unassigned { get; set; }
            public ReportOverviewObject Resolved { get; set; }

            public ReportOverview()
            {
                this.Opened = new ReportOverviewObject();
                this.Unattended = new ReportOverviewObject();
                this.Unassigned = new ReportOverviewObject();
                this.Resolved = new ReportOverviewObject();
            }
        }

        public class ReportOverviewObject
        {
            public int Total { get; set; }
            public int UpDown { get; set; }
        }



        public class ListPerformentMonitor
        {
            public ObjectPerformance Total { get; set; }
            public ObjectPerformance Incoming { get; set; }
            public ObjectPerformance Outgoing { get; set; }
            public ObjectPerformance Resolved { get; set; }
            public ObjectPerformance ResponeTime { get; set; }
            public ObjectPerformance ResolTime { get; set; }
        }
        public class ObjectPerformance
        {
            public int SumReport { get; set; }
            public List<ObjectReport> Report { get; set; }
        }

        public class ObjectReport
        {
            public string key { get; set; }
            public int value { get; set; }
        }

        public class ObjectReportTime
        {
            public string date { get; set; }
            public double value { get; set; }
        }
    }
}