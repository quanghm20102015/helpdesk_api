using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class ContactNote
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
        public string note
        {
            get;
            set;
        }
        public DateTime? timeNote
        {
            get;
            set;
        }
    }
}
