using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;
using static Interfaces.Model.EmailInfoLabel.ReportOverviewResponse;

namespace Interfaces.Model.EmailInfoLabel
{
    public class TopConversationGroupRespone : BaseResponse<ResponseStatus>
    {
        public List<Object> Result { get; set; }
        public class TopConversationAGroupObject
        {
            public string groupName { get; set; }
            public string totalMember { get; set; }
            public string totalConversation { get; set; }
        }
    }
}