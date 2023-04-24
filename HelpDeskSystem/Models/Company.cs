using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Company
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? companyName
        {
            get;
            set;
        }
    }
}
