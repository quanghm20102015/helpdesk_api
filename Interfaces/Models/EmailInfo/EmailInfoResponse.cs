using Interfaces.Base;
using Interfaces.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interfaces.Model.Account
{
    public class EmailInfoResponse : BaseResponse<ResponseStatus>
    {
        public Object emailInfo { get; set; }
        public List<LabelDetail> listLabel { get; set; }
        public List<Object> listAccount { get; set; }
        public List<Object> listEmailInfo { get; set; }        

    }
    public class EmailInfoDetail
    {
        public int id
        {
            get;
            set;
        }
        public int? idConfigEmail
        {
            get;
            set;
        }
        public string? messageId
        {
            get;
            set;
        }
        public DateTime? date
        {
            get;
            set;
        }
        public string? from
        {
            get;
            set;
        }
        public string? fromName
        {
            get;
            set;
        }
        public string? to
        {
            get;
            set;
        }
        public string? cc
        {
            get;
            set;
        }
        public string? bcc
        {
            get;
            set;
        }
        public string? subject
        {
            get;
            set;
        }
        public string? textBody
        {
            get;
            set;
        }
        public int? status
        {
            get;
            set;
        }
        public int? assign
        {
            get;
            set;
        }
        public int? idCompany
        {
            get;
            set;
        }
        public int? idLabel
        {
            get;
            set;
        }
        public string? idGuId
        {
            get;
            set;
        }
    }


    public class LabelDetail
    {
        public int id
        {
            get;
            set;
        }
        public string? name
        {
            get;
            set;
        }
        public string? description
        {
            get;
            set;
        }
        public string? color
        {
            get;
            set;
        }
        public int idCompany
        {
            get;
            set;
        }
        public bool check
        {
            get;
            set;
        }
    }
}