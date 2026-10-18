using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Requests
{
    public class TakeChapterRequest
    {
        [MaxLength(100)] public string? GuestName { get; set; }   // רלוונטי רק לאורח

    }
}
