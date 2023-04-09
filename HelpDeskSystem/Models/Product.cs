using System.ComponentModel.DataAnnotations;
namespace HelpDeskSystem.Models
{
    public class Product
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
        public string? brand
        {
            get;
            set;
        }
        public string? size
        {
            get;
            set;
        }
        public decimal price
        {
            get;
            set;
        }
        //public virtual ICollection<Order> orders
        //{
        //    get;
        //    set;
        //}
    }
}
