using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Account
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? fullname
        {
            get;
            set;
        }
        public string? company
        {
            get;
            set;
        }
        public string? workemail
        {
            get;
            set;
        }
        public string? password
        {
            get;
            set;
        }
        public int idCompany
        {
            get;
            set;
        }
        public bool confirm
        {
            get;
            set;
        }
        public bool login
        {
            get;
            set;
        }
        public int status
        {
            get;
            set;
        }
        public string? idGuId
        {
            get;
            set;
        }
        public string? avatar
        {
            get;
            set;
        }
        public string? fileName
        {
            get;
            set;
        }
        public int? role
        {
            get;
            set;
        }
        public bool isDelete
        {
            get;
            set;
        }
        public string? signature
        {
            get;
            set;
        }
        public int? language
        {
            get;
            set;
        }
        public string? displayName
        {
            get;
            set;
        }
        public int? numberOfDay
        {
            get;
            set;
        }
    }
}
