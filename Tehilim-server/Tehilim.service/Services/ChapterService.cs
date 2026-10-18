using AutoMapper;
using Microsoft.Extensions.Logging;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Exceptions;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Interfaces.Services;
using Tehilim.Core.Results;

namespace Tehilim.Service.Services
{
    public class ChapterService : IChapterService
    {
        private const string ChapterTakenMessage = "הפרק נלקח זה עתה על ידי משתתף אחר, אנא בחר פרק אחר";

        private readonly IChapterRepository _chapters;
        private readonly IBookRepository _books;
        private readonly IMapper _mapper;
        private readonly ILogger<ChapterService> _logger;

        public ChapterService(
            IChapterRepository chapters,
            IBookRepository books,
            IMapper mapper,
            ILogger<ChapterService> logger)
        {
            _chapters = chapters;
            _books = books;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<ChapterDto>>> GetByBookAsync(int bookId, CancellationToken ct)
        {
            // רשימה ריקה אינה מבדילה בין "אין ספר" ל"אין פרקים", לכן בודקים קיום בנפרד
            if (!await _books.ExistsAsync(bookId, ct))
                return Result<IReadOnlyList<ChapterDto>>.NotFound("הספר לא נמצא");

            var chapters = await _chapters.GetByBookAsync(bookId, ct);
            var dtos = _mapper.Map<IReadOnlyList<ChapterDto>>(chapters);
            return Result<IReadOnlyList<ChapterDto>>.Success(dtos);
        }

        public async Task<Result<ChapterDto>> TakeAsync(
            int bookId, int chapterNumber, int? userId, TakeChapterRequest request, CancellationToken ct)
        {
            // 1. טעינה עם tracking — בלי זה ה-Version לא ייבדק בשמירה
            var chapter = await _chapters.GetForUpdateAsync(bookId, chapterNumber, ct);
            if (chapter is null)
                return Result<ChapterDto>.NotFound("הפרק לא נמצא");

            // 2. בדיקות עסקיות על אותו אובייקט שיישמר
            if (chapter.Book.Status != BookStatus.Open)
                return Result<ChapterDto>.Conflict("הספר כבר הושלם");
           
            if (chapter.Status != ChapterStatus.Available)
            {
                _logger.LogWarning(
                    "Chapter {ChapterNumber} in book {BookId} is already taken",
                    chapterNumber, bookId);
                return Result<ChapterDto>.Conflict(ChapterTakenMessage);
            }

            // 3. עדכון: משתמש מחובר או אורח (שם האורח אופציונלי)
            chapter.Status = ChapterStatus.Taken;
            chapter.TakenAt = DateTime.UtcNow;
            if (userId is not null)
            {
                chapter.TakenByUserId = userId;
                chapter.TakenByName = null;
            }
            else
            {
                chapter.TakenByUserId = null;
                chapter.TakenByName = string.IsNullOrWhiteSpace(request.GuestName)
                    ? null
                    : request.GuestName.Trim();
            }

            // 4. שמירה; המאחר מבין שניים מקבל ConcurrencyConflictException מה-Repository
            try
            {
                await _chapters.SaveChangesAsync(ct);
            }
            catch (ConcurrencyConflictException)
            {
                _logger.LogWarning(
                    "Concurrency conflict taking chapter {ChapterNumber} in book {BookId}",
                    chapterNumber, bookId);
                return Result<ChapterDto>.Conflict(ChapterTakenMessage);
            }

            // 5. אחרי ה-commit: סגירת הספר אם לא נשארו פרקים פנויים.
            // CancellationToken.None — הפרק כבר נשמר, ואסור שביטול הבקשה ישאיר ספר מלא במצב "פתוח".
            await _books.CompleteIfNoAvailableChaptersAsync(bookId, CancellationToken.None);

            return Result<ChapterDto>.Success(_mapper.Map<ChapterDto>(chapter));
        }
    }
}