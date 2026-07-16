using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.Role;

namespace Frontend.Services;

public class RoleService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public RoleService(HttpClient http, ILocalStorageService localStorage)
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
    
    public async Task<List<RoleDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<RoleDto>>("api/Role");
    }

    public async Task<bool> CreateAsync(CreateRoleDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/Role/create", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid roleId)
    {
        await EnsureBearerAsync();
        var res = await _http.DeleteAsync($"api/Role?roleId={roleId}");
        return res.IsSuccessStatusCode;
    }
}