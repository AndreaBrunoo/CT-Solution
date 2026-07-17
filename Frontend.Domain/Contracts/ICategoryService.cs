using Frontend.Domain.Dtos.Category;

namespace Frontend.Domain.Contracts;

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CategoryDto>?> GetAllAsync();
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
}