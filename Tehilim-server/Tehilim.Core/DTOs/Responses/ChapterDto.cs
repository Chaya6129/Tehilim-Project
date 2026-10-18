using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Responds
{
    public record ChapterDto(
     int Number,
     string Status,
     string? TakenByName,              // שם המשתמש או שם האורח, מאוחד לשדה אחד
     DateTime? TakenAt);
}
