using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.DTOs.Responds
{
    public record BookListItemDto(        // שורה ברשימת ספרים
       int Id,
       string Goal,
       bool IsPublic,
       string Status,
       string OwnerName,
       int AvailableChapters,            // מחושב בשאילתה, לא נטען כרשימה
       DateTime CreatedAt,
       IReadOnlyList<CategoryDto> Categories);
}