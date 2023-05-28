using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class LabelDistributionResponse : BaseResponse<ResponseStatus>
    {
        public List<LabelDistribution> result { get; set; }
        public List<string> colors { get; set; }
        public List<LabelDistributionTable> resultTable { get; set; }
        public List<TopTrendingLabel> topTrending { get; set; }

        //public ListPerformentMonitor reportData { get; set; }

        public class LabelDistribution
        {
            public string Name { get; set; }
            public decimal Y { get; set; }
        }
        public class LabelDistributionTable
        {
            public string Name { get; set; }
            public string ClassName { get; set; }
            public string Distribution { get; set; }
            public string Conversation { get; set; }
        }
        public class TopTrendingLabel
        {
            public string Name { get; set; }
            public string UserTime { get; set; }
            public string Conversation { get; set; }
        }
    }
}