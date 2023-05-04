using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Csat
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public int idEmailInfo
        {
            get;
            set;
        }
        public int idFeedBack
        {
            get;
            set;
        }
        public string? descriptionFeedBack
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
