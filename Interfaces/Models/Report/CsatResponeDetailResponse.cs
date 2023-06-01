using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class CsatResponeDetailResponse : BaseResponse<ResponseStatus>
    {
        public List<Object> result { get; set; }
        public int total { get; set; }

    }
}