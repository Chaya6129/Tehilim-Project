using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;

namespace Tehilim.Core.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct);
        Task<Category?> GetByIdForUpdateAsync(int id, CancellationToken ct);
        Task<List<Category>> GetByIdsForUpdateAsync(IReadOnlyCollection<int> ids, CancellationToken ct);
        Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken ct);
        Task AddAsync(Category category, CancellationToken ct);
        void Remove(Category category);
        Task SaveChangesAsync(CancellationToken ct);
    }
}