﻿using System.ComponentModel.DataAnnotations;

namespace WebBasedInstaller.Models
{
    public class DTOConnectionSetting
    {
        [Key]
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}