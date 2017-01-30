using System.ComponentModel.DataAnnotations;

namespace WebBasedInstaller.Models
{
    public class User
    {
        [Key]
        public string UserName { get; set; }
    }
}