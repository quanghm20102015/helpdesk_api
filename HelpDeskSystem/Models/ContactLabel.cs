using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class ContactLabel
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public int? idContact
        {
            get;
            set;
        }
        public int? idLabel
        {
            get;
            set;
        }
    }
}
