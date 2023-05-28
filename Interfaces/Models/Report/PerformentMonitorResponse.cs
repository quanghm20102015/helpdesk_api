using Interfaces.Base;
using Interfaces.Constants;
using System;
using System.Collections.Generic;
using static Interfaces.Model.EmailInfoLabel.ReportOverviewResponse;

namespace Interfaces.Model.EmailInfo
{
    public class PerformentMonitorResponse : BaseResponse<ResponseStatus>
    {
        public ObjectPerformentMonitor Result { get; set; }
        public class ObjectPerformentMonitor
        {
            public List<string> label { get; set; }
            public List<int> data { get; set; }

            public ObjectPerformentMonitor()
            {
                label = new List<string>();
                data = new List<int>();
            }
        }
    }
}