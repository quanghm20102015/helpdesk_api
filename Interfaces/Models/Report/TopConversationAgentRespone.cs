using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;
using static Interfaces.Model.EmailInfoLabel.ReportOverviewResponse;

namespace Interfaces.Model.EmailInfoLabel
{
    public class TopConversationAgentResponse : BaseResponse<ResponseStatus>
    {
        public List<TopConversationAgentObject> Result { get; set; }
        public class TopConversationAgentObject
        {
            public int IdUser { get; set; }
            public string Agent { get; set; }
            public string Mail { get; set; }
            public int Open { get; set; }
            public int Unattended { get; set; }
        }
    }
}