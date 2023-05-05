using Interfaces.Base;
using Interfaces.Constants;
using System;
using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class EmailInfoCountByCAResponse : BaseResponse<ResponseStatus>
    {
        public int All { get; set; }
        public int ByAgent { get; set; }
    }
}