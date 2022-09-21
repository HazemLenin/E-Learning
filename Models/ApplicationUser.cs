﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Learning.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public double? Wallet { get; set; } = 0.0;
    }
}
