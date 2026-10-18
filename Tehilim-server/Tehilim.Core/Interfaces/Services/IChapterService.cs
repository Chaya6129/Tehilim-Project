using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Results;

namespace Tehilim.Core.Interfaces.Services
{

    public interface IChapterService
    {
        Task<Result<IReadOnlyList<ChapterDto>>> GetByBookAsync(int bookId, CancellationToken ct);  // 404 אם הספר לא קיים
        Task<Result<ChapterDto>> TakeAsync(
            int bookId, int chapterNumber, int? userId, TakeChapterRequest request, CancellationToken ct);  // 404, 409
    }
}
