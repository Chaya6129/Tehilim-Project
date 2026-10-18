using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tehilim.Core.DTOs;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;

namespace Tehilim.Core.Interfaces.Repositories
{
    public interface IBookRepository
    {
        // סינון+חיפוש+מיון+דפדוף
        Task<Book?> GetByIdAsync(int id, CancellationToken ct);            // קריאה, AsNoTracking
        Task<Book?> GetByIdForUpdateAsync(int id, CancellationToken ct);   // עדכון/מחיקה, tracking
        Task AddAsync(Book book, CancellationToken ct);                    // הספר + 150 פרקים יחד
        void Remove(Book book);
        Task SaveChangesAsync(CancellationToken ct);
        Task<(IReadOnlyList<BookListItemDto> Items, int TotalCount)> GetPublicOpenAsync(
    BookQueryParameters query, CancellationToken ct);

        Task<IReadOnlyList<BookListItemDto>> GetByOwnerAsync(int ownerId, CancellationToken ct);
        Task<bool> CompleteIfNoAvailableChaptersAsync(int bookId, CancellationToken ct); //
        Task<bool> ExistsAsync(int id, CancellationToken ct);   // האם הספר קייםסוגר רק אם אין פרקים פנויים
    }
}
