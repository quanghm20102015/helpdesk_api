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
        public string? extension
        {
            get;
            set;
        }
        public string? sizeText
        {
            get;
            set;
        }
        public string? fileType
        {
            get;
            set;
        }
        public string? name
        {
            get;
            set;
        }
        public string? part
        {
            get;
            set;
        }
    }
}
