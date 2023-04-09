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
    }
}
