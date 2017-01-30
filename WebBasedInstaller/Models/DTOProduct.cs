using System.ComponentModel.DataAnnotations;

namespace WebBasedInstaller.Models
{
    public class DTOProduct
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
    }
}