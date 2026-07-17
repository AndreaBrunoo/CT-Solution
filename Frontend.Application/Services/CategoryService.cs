using Frontend.Domain.Contracts;
using Frontend.Domain.Dtos.Category;
using Frontend.Infrastructure.Api;

namespace Frontend.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApiClient _api;

    public CategoryService(IApiClient api)
    {
        _api = api;
    }

    public Task<CategoryDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<CategoryDto>($"api/Category/{id}");

    public async Task<IReadOnlyList<CategoryDto>?> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<CategoryDto>>("api/Category");
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var result = await _api.PostAsync<CategoryDto>("api/Category/create", dto);
        return result!;
    }

    public async Task<CategoryDto?> UpdateAsync(UpdateCategoryDto dto)
    {
        return await _api.PutAsync<CategoryDto>("api/Category/update", dto);
    }

    public Task<bool> DeleteAsync(Guid id)
        => _api.DeleteAsync($"api/Category/delete?id={id}");
}