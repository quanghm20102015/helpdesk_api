using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class History
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        public int? idCompany
        {
            get;
            set;
        }
        public int? idDetail
        {
            get;
            set;
        }
        public int? type//1: EmailInfo
        {
            get;
            set;
        }
        public DateTime? time
        {
            get;
            set;
        }
        public string? content
        {
            get;
            set;
        }
        public string? fullName
        {
            get;
            set;
        }
    }
}
