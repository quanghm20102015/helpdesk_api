using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class TrafficResponse : BaseResponse<ResponseStatus>
    {
        public List<TrafficDay> categories { get; set; }
        public List<List<int>> data { get; set; }

        public class TrafficDay
        {
            public int id { get; set; }
            public string labels { get; set; }
            public string days { get; set; }
        }
    }
}