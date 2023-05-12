using Interfaces.Base;
using Interfaces.Constants;
using System;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfo
{
    public class EmailInfoGetMenuCountResponse : BaseResponse<ResponseStatus>
    {
        public EmailInfoCount emailInfoCount { get; set; }
        public class EmailInfoCount
        {
            public int All { get; set; }
            public int Mine { get; set; }
            public int Following { get; set; }
            public int Unassigned { get; set; }
            public int Resolved { get; set; }
            public int Trash { get; set; }
        }
    }
}