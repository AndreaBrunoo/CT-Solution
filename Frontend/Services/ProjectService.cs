using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.Project;

namespace Frontend.Services;

public class ProjectService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public ProjectService(HttpClient http, ILocalStorageService localStorage)
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

    public async Task<List<ProjectDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<ProjectDto>>("api/Project");
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<ProjectDto>($"api/Project/{id}");
    }

    public async Task<bool> CreateAsync(CreateProjectDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/Project/create", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(UpdateProjectDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PutAsJsonAsync("api/Project/update", dto);
        return res.IsSuccessStatusCode;
    }

        public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsureBearerAsync();
        var res = await _http.DeleteAsync($"api/Project/delete?id={id}");
        return res.IsSuccessStatusCode;
    }
}