using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class EmailInfoLabel
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public int? idEmailInfo
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
