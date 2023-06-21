using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class LabelGroup
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
        public string? description
        {
            get;
            set;
        }
        public string? color
        {
            get;
            set;
        }
        public int idCompany
        {
            get;
            set;
        }
    }
}
