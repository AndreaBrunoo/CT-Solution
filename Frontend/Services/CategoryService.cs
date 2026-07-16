using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.Category;

namespace Frontend.Services;

public class CategoryService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public CategoryService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    private async Task EnsureBearerAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<CategoryDto>($"api/Category/{id}");
    }

    public async Task<List<CategoryDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<CategoryDto>>("api/Category");
    }

    public async Task<bool> CreateAsync(CreateCategoryDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/Category/create", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(UpdateCategoryDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PutAsJsonAsync("api/Category/update", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsureBearerAsync();
        var res = await _http.DeleteAsync($"api/Category/delete?id={id}");
        return res.IsSuccessStatusCode;
    }
}