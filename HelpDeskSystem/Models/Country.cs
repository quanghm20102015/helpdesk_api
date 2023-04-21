using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Country
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? name
        {
            get;
            set;
        }
    }
}
