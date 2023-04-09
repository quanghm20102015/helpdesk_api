using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Order
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        public int product_id
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
        public string? address
        {
            get;
            set;
        }
        public string? phone
        {
            get;
            set;
        }
        public DateTime createdon
        {
            get;
            set;
        }
        public virtual Product product
        {
            get;
            set;
        }
    }
}
