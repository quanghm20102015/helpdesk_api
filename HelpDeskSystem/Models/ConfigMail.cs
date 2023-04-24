using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class ConfigMail
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? yourName
        {
            get;
            set;
        }
        public string? email
        {
            get;
            set;
        }
        public string? password
        {
            get;
            set;
        }
        public string? incoming
        {
            get;
            set;
        }
        public int? incomingPort
        {
            get;
            set;
        }
        public string? outgoing
        {
            get;
            set;
        }
        public int? outgoingPort
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
