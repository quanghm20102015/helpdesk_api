using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class CsatOverviewResponse : BaseResponse<ResponseStatus>
    {
        public CsatOverview result { get; set; }

        public class CsatOverview
        {
            public CsatOverviewObject SatisfactionScore { get; set; }
            public CsatOverviewObject ResponseRate { get; set; }
            public CsatOverviewObject TotalResponses { get; set; }

            public CsatOverview()
            {
                this.SatisfactionScore = new CsatOverviewObject();
                this.ResponseRate = new CsatOverviewObject();
                this.TotalResponses = new CsatOverviewObject();
            }
        }

        public class CsatOverviewObject
        {
            public int Total { get; set; }
            public int UpDown { get; set; }
        }
    }
}