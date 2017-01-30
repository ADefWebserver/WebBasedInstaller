using System.ComponentModel.DataAnnotations;

namespace WebBasedInstaller.Models
{
    public class DTOStatus
    {
        [Key]
        public string StatusMessage { get; set; }
        public bool Success { get; set; }
    }
}