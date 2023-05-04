﻿using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;

namespace Interfaces.Model.Account
{
    public class SignupResponse : BaseResponse<ResponseStatus>
    {
        public int Id { get; set; }
        public string idGuId { get; set; }
    }
}