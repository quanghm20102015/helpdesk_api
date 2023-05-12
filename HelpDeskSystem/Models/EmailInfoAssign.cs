using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class EmailInfoAssign
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
        public int? idUser
        {
            get;
            set;
        }
    }
}
