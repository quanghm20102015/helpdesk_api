﻿using System.Collections.Generic;

namespace Interfaces.Model.EmailInfoLabel
{
    public class EmailInfoLabelRequest
    {
        public int id { get; set; }
        public int idEmailInfo { get; set; }
        public List<int> listLabel { get; set; }
    }
}