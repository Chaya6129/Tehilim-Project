using Microsoft.EntityFrameworkCore;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Data.Context;

namespace Tehilim.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) => _context = context;

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct) =>
            await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto(c.Id, c.Name))
                .ToListAsync(ct);

        public Task<Category?> GetByIdForUpdateAsync(int id, CancellationToken ct) =>
            _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task<List<Category>> GetByIdsForUpdateAsync(IReadOnlyCollection<int> ids, CancellationToken ct) =>
            _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync(ct);

        public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken ct) =>
            _context.Categories.AnyAsync(c => c.Name == name && c.Id != excludeId, ct);

        public async Task AddAsync(Category category, CancellationToken ct) =>
            await _context.Categories.AddAsync(category, ct);

        public void Remove(Category category) => _context.Categories.Remove(category);

        public Task SaveChangesAsync(CancellationToken ct) => _context.SaveChangesAsync(ct);
    }
}