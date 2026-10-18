using AutoMapper;
using Tehilim.Core.DTOs;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Interfaces.Services;
using Tehilim.Core.Results;

namespace Tehilim.Service.Services
{
    public class BookService : IBookService
    {
        private const int ChaptersPerBook = 150;
        private const string InvalidCategoriesMessage = "אחת או יותר מהקטגוריות שנבחרו אינה קיימת";

        private readonly IBookRepository _books;
        private readonly ICategoryRepository _categories;
        private readonly IMapper _mapper;

        public BookService(IBookRepository books, ICategoryRepository categories, IMapper mapper)
        {
            _books = books;
            _categories = categories;
            _mapper = mapper;
        }

        public async Task<PagedResult<BookListItemDto>> GetPublicOpenAsync(BookQueryParameters query, CancellationToken ct)
        {
            // הסינון, המיון והדפדוף כולם בתוך השאילתה ב-Repository
            var (items, totalCount) = await _books.GetPublicOpenAsync(query, ct);

            return new PagedResult<BookListItemDto>(
                Items: items,
                TotalCount: totalCount,
                Page: query.Page,
                PageSize: query.PageSize);
        }

        public Task<IReadOnlyList<BookListItemDto>> GetMyBooksAsync(int userId, CancellationToken ct)
            => _books.GetByOwnerAsync(userId, ct);

        public async Task<Result<BookDetailsDto>> GetByIdAsync(int id, CancellationToken ct)
        {
            // נגיש לכולם, גם ספר לא פומבי — גישה בקישור ישיר
            var book = await _books.GetByIdAsync(id, ct);
            return book is null
                ? Result<BookDetailsDto>.NotFound("הספר לא נמצא")
                : Result<BookDetailsDto>.Success(_mapper.Map<BookDetailsDto>(book));
        }

        public async Task<Result<BookDetailsDto>> CreateAsync(CreateBookRequest request, int ownerId, CancellationToken ct)
        {
            var categories = await LoadCategoriesAsync(request.CategoryIds, ct);
            if (categories is null)
                return Result<BookDetailsDto>.Validation(InvalidCategoriesMessage);

            var book = _mapper.Map<Book>(request);   // יעד ופומביות מהבקשה
            book.OwnerId = ownerId;
            book.Status = BookStatus.Open;
            book.CreatedAt = DateTime.UtcNow;
            book.CompletedAt = null;
            book.Categories = categories;
            book.Chapters = Enumerable.Range(1, ChaptersPerBook)
                .Select(n => new Chapter
                {
                    Number = n,
                    Status = ChapterStatus.Available,
                    Version = Guid.NewGuid()
                })
                .ToList();

            await _books.AddAsync(book, ct);
            await _books.SaveChangesAsync(ct);

            // טעינה מחדש, כדי שהבעלים והקטגוריות ייכללו בתגובה
            var created = await _books.GetByIdAsync(book.Id, ct);
            return Result<BookDetailsDto>.Success(_mapper.Map<BookDetailsDto>(created!));
        }

        public async Task<Result> UpdateAsync(int id, UpdateBookRequest request, int userId, CancellationToken ct)
        {
            var book = await _books.GetByIdForUpdateAsync(id, ct);
            if (book is null)
                return Result.NotFound("הספר לא נמצא");

            if (book.OwnerId != userId)
                return Result.Forbidden("רק יוצר הספר רשאי לעדכן אותו");

            var categories = await LoadCategoriesAsync(request.CategoryIds, ct);
            if (categories is null)
                return Result.Validation(InvalidCategoriesMessage);

            // ה-Profile ממפה יעד ופומביות בלבד — שאר השדות לא נוגעים
            _mapper.Map(request, book);

            book.Categories.Clear();
            book.Categories.AddRange(categories);

            await _books.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, int userId, bool isAdmin, CancellationToken ct)
        {
            var book = await _books.GetByIdForUpdateAsync(id, ct);
            if (book is null)
                return Result.NotFound("הספר לא נמצא");

            if (book.OwnerId != userId && !isAdmin)
                return Result.Forbidden("רק יוצר הספר או מנהל רשאים למחוק אותו");

            _books.Remove(book);   // הפרקים נמחקים במחיקה מדורגת
            await _books.SaveChangesAsync(ct);

            return Result.Success();
        }

        private async Task<List<Category>?> LoadCategoriesAsync(List<int> ids, CancellationToken ct)
        {
            var distinctIds = ids.Distinct().ToList();
            if (distinctIds.Count == 0)
                return new List<Category>();

            var categories = await _categories.GetByIdsForUpdateAsync(distinctIds, ct);
            return categories.Count == distinctIds.Count ? categories : null;
        }
    }
}