using System.ComponentModel.DataAnnotations;

namespace WebBasedInstaller.Models
{
    public class DTOVersion
    {
        [Key]
        public string VersionNumber { get; set; }
        public bool isNewDatabase { get; set; }
        public bool isUpToDate { get; set; }
    }
}