using Microsoft.EntityFrameworkCore;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;
using Tehilim.Core.Exceptions;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Data.Context;

namespace Tehilim.Data.Repositories;

public class ChapterRepository : IChapterRepository
{
    private readonly AppDbContext _context;

    public ChapterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Chapter>> GetByBookAsync(int bookId, CancellationToken ct) =>
        await _context.Chapters
                 .AsNoTracking()
                 .Include(c => c.TakenByUser)
                 .Where(c => c.BookId == bookId)
                 .OrderBy(c => c.Number)
                 .ToListAsync(ct);

    public Task<Chapter?> GetForUpdateAsync(int bookId, int chapterNumber, CancellationToken ct) =>
        _context.Chapters
            .Include(c => c.Book)
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.Number == chapterNumber, ct);


    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException(
                "Chapter was modified by another request.", ex);
        }
    }
}