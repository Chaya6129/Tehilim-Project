using AutoMapper;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;
using Tehilim.Core.Interfaces.Repositories;
using Tehilim.Core.Interfaces.Services;
using Tehilim.Core.Results;

namespace Tehilim.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private const string NameTakenMessage = "קטגוריה בשם זה כבר קיימת";
        private const string NotFoundMessage = "הקטגוריה לא נמצאה";

        private readonly ICategoryRepository _categories;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categories, IMapper mapper)
        {
            _categories = categories;
            _mapper = mapper;
        }

        public Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct)
            => _categories.GetAllAsync(ct);

        public async Task<Result<CategoryDto>> CreateAsync(CategoryRequest request, CancellationToken ct)
        {
            var name = request.Name.Trim();

            if (await _categories.NameExistsAsync(name, excludeId: null, ct))
                return Result<CategoryDto>.Conflict(NameTakenMessage);

            var category = new Category { Name = name };
            await _categories.AddAsync(category, ct);
            await _categories.SaveChangesAsync(ct);

            return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category));
        }

        public async Task<Result> UpdateAsync(int id, CategoryRequest request, CancellationToken ct)
        {
            var category = await _categories.GetByIdForUpdateAsync(id, ct);
            if (category is null)
                return Result.NotFound(NotFoundMessage);

            var name = request.Name.Trim();

            if (await _categories.NameExistsAsync(name, excludeId: id, ct))
                return Result.Conflict(NameTakenMessage);

            category.Name = name;
            await _categories.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct)
        {
            var category = await _categories.GetByIdForUpdateAsync(id, ct);
            if (category is null)
                return Result.NotFound(NotFoundMessage);

            _categories.Remove(category);   // הקישורים לספרים נמחקים במחיקה מדורגת; הספרים עצמם נשארים
            await _categories.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}