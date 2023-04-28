using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Team
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
        public int idCompany
        {
            get;
            set;
        }
        public string? description
        {
            get;
            set;
        }
    }
}
