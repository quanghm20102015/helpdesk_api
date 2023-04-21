using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Contact
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
        public string? email
        {
            get;
            set;
        }
        public string? bio
        {
            get;
            set;
        }
        public string? phoneNumber
        {
            get;
            set;
        }
        public string? company
        {
            get;
            set;
        }
        public int? country
        {
            get;
            set;
        }
        public string? city
        {
            get;
            set;
        }
        public string? facebook
        {
            get;
            set;
        }
        public string? twitter
        {
            get;
            set;
        }
        public string? linkedin
        {
            get;
            set;
        }
        public string? github
        {
            get;
            set;
        }
    }
}
