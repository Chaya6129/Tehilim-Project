using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Requests
{
    public class UpdateBookRequest
    {
        [Required, MaxLength(200)] public string Goal { get; set; } = "";
        public bool IsPublic { get; set; }
        public List<int> CategoryIds { get; set; } = new();
    }
}
