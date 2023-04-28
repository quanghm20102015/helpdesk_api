using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class TeamAgent
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public int idAgent
        {
            get;
            set;
        }
    }
}
