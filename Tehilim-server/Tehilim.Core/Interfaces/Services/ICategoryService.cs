using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Results;

namespace Tehilim.Core.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct);
        Task<Result<CategoryDto>> CreateAsync(CategoryRequest request, CancellationToken ct);   // 409 אם השם קיים
        Task<Result> UpdateAsync(int id, CategoryRequest request, CancellationToken ct);       // 404, 409
        Task<Result> DeleteAsync(int id, CancellationToken ct);                               // 404
    }
}