using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class EmailInfo
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
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
        //1: in; 2: out
        public int type
        {
            get;
            set;
        }
        public bool isDelete
        {
            get;
            set;
        }
        public int idUserDelete
        {
            get;
            set;
        }
        public DateTime? dateDelete
        {
            get;
            set;
        }
    }
}
