using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required, MaxLength(100)] public string FullName { get; set; } = "";
        [Required, EmailAddress, MaxLength(200)] public string Email { get; set; } = "";
        [Required, MinLength(6), MaxLength(100)] public string Password { get; set; } = "";
    }
}
