using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Learning.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DefaultValue(0.0)]
        public double? Wallet { get; set; }
    }
}
