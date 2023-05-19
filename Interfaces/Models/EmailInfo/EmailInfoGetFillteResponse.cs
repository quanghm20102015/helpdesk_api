using Interfaces.Base;
using Interfaces.Constants;
using System;
using System.Collections.Generic;

namespace Interfaces.Model.EmailInfo
{
    public class EmailInfoGetFillteResponse : BaseResponse<ResponseStatus>
    {
        //public int All { get; set; }
        //public int Mine { get; set; }
        //public int Following { get; set; }
        //public int Unassigned { get; set; }
        //public int Resolved { get; set; }
        //public int Trash { get; set; }
        //public int ByAgent { get; set; }
        public List<dynamic> listEmailInfo { get; set; }
        public int total { get; set; }
    }
}