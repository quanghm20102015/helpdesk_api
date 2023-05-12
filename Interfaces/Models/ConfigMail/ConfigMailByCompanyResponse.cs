using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interfaces.Model.ConfigMail
{
    public class ConfigMailByCompanyResponse : BaseResponse<ResponseStatus>
    {
        public List<ConfigMailDetail> listConfigMail { get; set; }

    }
    public class ConfigMailDetail
    {
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? yourName
        {
            get;
            set;
        }
        public string? email
        {
            get;
            set;
        }
        public int idCompany
        {
            get;
            set;
        }
        public int countEmail
        {
            get;
            set;
        }
    }
}