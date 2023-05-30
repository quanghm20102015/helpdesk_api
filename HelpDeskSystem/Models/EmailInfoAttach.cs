using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class EmailInfoAttach
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
        public string? pathFile
        {
            get;
            set;
        }
        public string? fileName
        {
            get;
            set;
        }
    }
}
