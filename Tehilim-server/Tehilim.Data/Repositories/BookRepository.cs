using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tehilim.Core.DTOs;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Data.Context;

namespace Tehilim.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    // מספר הפרקים הפנויים - מחושב בבסיס הנתונים, משמש למיון
    private static readonly Expression<Func<Book, int>> AvailableCount =
        b => b.Chapters.Count(c => c.Status == ChapterStatus.Available);

    // הקרנה ל-DTO - מתורגמת ל-SQL, בלי לטעון ישויות
    private static readonly Expression<Func<Book, BookListItemDto>> ToListItem =
      b => new BookListItemDto(
          b.Id,
          b.Purpose,
          b.IsPublic,
          b.Status.ToString(),
          b.Owner.DisplayName,
          b.Chapters.Count(c => c.Status == ChapterStatus.Available),
          b.CreatedAt,
          b.Categories.OrderBy(c => c.Name).Select(c => new CategoryDto(c.Id, c.Name)).ToList());

    public async Task<(IReadOnlyList<BookListItemDto> Items, int TotalCount)> GetPublicOpenAsync(
        BookQueryParameters query, CancellationToken ct)
    {
        var books = _context.Books
            .AsNoTracking()
            .Where(b => b.IsPublic && b.Status == BookStatus.Open);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            books = books.Where(b => b.Purpose.Contains(search));
        }
        if (query.CategoryId is int categoryId)
            books = books.Where(b => b.Categories.Any(c => c.Id == categoryId));

        var totalCount = await books.CountAsync(ct);

        IOrderedQueryable<Book> ordered = query.SortBy switch
        {
            BookSortBy.AvailableChapters => query.Descending
                ? books.OrderByDescending(AvailableCount)
                : books.OrderBy(AvailableCount),
            _ => query.Descending
                ? books.OrderByDescending(b => b.CreatedAt)
                : books.OrderBy(b => b.CreatedAt)
        };

        var items = await ordered
            .ThenBy(b => b.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToListItem)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<BookListItemDto>> GetByOwnerAsync(int ownerId, CancellationToken ct) =>
        await _context.Books
            .AsNoTracking()
            .Where(b => b.OwnerId == ownerId)
            .OrderByDescending(b => b.CreatedAt)
            .ThenBy(b => b.Id)
            .Select(ToListItem)
            .ToListAsync(ct);

    public Task<Book?> GetByIdAsync(int id, CancellationToken ct) =>
    _context.Books
        .AsNoTracking()
        .Include(b => b.Owner)
        .Include(b => b.Chapters.OrderBy(c => c.Number))
            .ThenInclude(c => c.TakenByUser)
        .Include(b => b.Categories.OrderBy(c => c.Name))
        .AsSplitQuery()
        .FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<Book?> GetByIdForUpdateAsync(int id, CancellationToken ct) =>
        _context.Books
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    public async Task AddAsync(Book book, CancellationToken ct) =>
        await _context.Books.AddAsync(book, ct);

    public void Remove(Book book) =>
        _context.Books.Remove(book);

    public Task SaveChangesAsync(CancellationToken ct) =>
        _context.SaveChangesAsync(ct);
    public async Task<bool> CompleteIfNoAvailableChaptersAsync(int bookId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var affected = await _context.Books
            .Where(b => b.Id == bookId
                     && b.Status == BookStatus.Open
                     && !b.Chapters.Any(c => c.Status == ChapterStatus.Available))
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Status, BookStatus.Completed)
                .SetProperty(b => b.CompletedAt, now), ct);

        return affected > 0;
    }
    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
    _context.Books.AnyAsync(b => b.Id == id, ct);
}