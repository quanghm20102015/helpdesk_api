using Interfaces.Base;
using Interfaces.Constants;
using System;
using System.Collections.Generic;
using static Interfaces.Model.EmailInfoLabel.ReportOverviewResponse;

namespace Interfaces.Model.EmailInfo
{
    public class PerformentMonitorTotalResponse : BaseResponse<ResponseStatus>
    {
        public ObjectPerformentMonitorTotal Result { get; set; }
        public class ObjectPerformentMonitorTotal
        {
            public int Total { get; set; }
            public int Incoming { get; set; }
            public int Outgoing { get; set; }
            public int Resolved { get; set; }
            public int ResponeTime { get; set; }
            public int ResolveTime { get; set; }
        }
    }
}