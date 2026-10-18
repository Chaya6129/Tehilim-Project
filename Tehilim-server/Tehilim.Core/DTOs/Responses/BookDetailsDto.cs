using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Responds
{
    public record BookDetailsDto(         // ספר מלא, למסך הספר
     int Id,
     string Goal,
     bool IsPublic,
     string Status,
     int OwnerId,                      // כדי שהלקוח ידע אם להציג "עריכה"
     string OwnerName,
     DateTime CreatedAt,
     DateTime? CompletedAt,
     IReadOnlyList<ChapterDto> Chapters,
    IReadOnlyList<CategoryDto> Categories);
}
