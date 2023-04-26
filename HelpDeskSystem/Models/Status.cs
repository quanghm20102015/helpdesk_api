using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Status
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? statusName
        {
            get;
            set;
        }
    }
}
