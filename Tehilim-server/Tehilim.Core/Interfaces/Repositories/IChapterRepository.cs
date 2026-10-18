using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tehilim.Core.Entities;

namespace Tehilim.Core.Interfaces.Repositories
{
    public interface IChapterRepository
    {
        Task<IReadOnlyList<Chapter>> GetByBookAsync(int bookId, CancellationToken ct);
        Task<Chapter?> GetForUpdateAsync(int bookId, int chapterNumber, CancellationToken ct); // tracking + Include(Book)
        Task SaveChangesAsync(CancellationToken ct);
    }
}
