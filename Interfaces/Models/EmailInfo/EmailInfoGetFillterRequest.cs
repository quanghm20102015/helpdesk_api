using System.Collections.Generic;

namespace Interfaces.Model.EmailInfo
{
    public class EmailInfoGetFillterRequest
    {
        public int status { get; set; }
        public int assign { get; set; }
        public int idConfigEmail { get; set; }
        public int idLabel { get; set; }
        public int idCompany { get; set; }
        public string textSearch { get; set; }
        public int idUserFollow { get; set; }
        public int idUserTrash { get; set; }
        public bool unAssign { get; set; }

    }
}