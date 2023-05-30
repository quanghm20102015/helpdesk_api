using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class CsatResponeDistributionResponse : BaseResponse<ResponseStatus>
    {
        public List<CsatResponeDistribution> result { get; set; }
        public List<string> colors { get; set; }
        public List<CsatResponeDistributionTable> resultTable { get; set; }
        public int total { get; set; }

        public class CsatResponeDistribution
        {
            public string Name { get; set; }
            public decimal Y { get; set; }
        }

        public class CsatResponeDistributionTable
        {
            public string ClassName { get; set; }
            public string Distribution { get; set; }
            public string Conversation { get; set; }
        }
    }
}