using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.User;

namespace Frontend.Services;

public class UserService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public UserService(HttpClient http, ILocalStorageService localStorage)
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

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/User/register", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("/api/User/login", dto);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (result?.Token != null)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
        }

        return result;
    }

    public async Task<List<UserDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<UserDto>>("api/User");
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<UserDto>($"api/User/{id}");
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<UserDto>($"api/User/email/{email}");
    }

    public async Task<bool> AssignRoleAsync(Guid userId, Guid roleId)
    {
        await EnsureBearerAsync();

        var res = await _http.PostAsync(
            $"api/User/{userId}/roles/{roleId}",
            null
        );

        return res.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId)
    {
        await EnsureBearerAsync();

        var res = await _http.PostAsync(
            $"api/User/{userId}/roles/{roleId}",
            null
        );

        return res.IsSuccessStatusCode;
    }

    public async Task<bool> HasRoleAsync(Guid userId, string roleName)
    {
        await EnsureBearerAsync();

        var result = await _http.GetFromJsonAsync<RoleCheckResponse>(
            $"api/User/{userId}/roles/check?roleName={roleName}"
        );

        return result?.HasRole ?? false;
    }

    public class RoleCheckResponse
    {
        public bool HasRole { get; set; }
    }

    public async Task<bool> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto)
    {
        await EnsureBearerAsync();

        var res = await _http.PutAsJsonAsync(
            $"api/User/{userId}/password",
            dto
        );

        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        await EnsureBearerAsync();

        var res = await _http.DeleteAsync($"api/User/{userId}");

        return res.IsSuccessStatusCode;
    }
}