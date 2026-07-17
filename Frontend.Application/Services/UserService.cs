using Frontend.Domain.Contracts;
using Frontend.Domain.Dtos.User;
using Frontend.Infrastructure.Api;

namespace Frontend.Application.Services;

public class UserService : IUserService
{
    private readonly IApiClient _api;

    public UserService(IApiClient api)
    {
        _api = api;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        return await _api.PostAsync<AuthResponseDto>("api/User/login", dto);
    }

    public async Task<UserDto?> RegisterAsync(RegisterDto dto)
    {
        return await _api.PostAsync<UserDto>("api/User/register", dto);
    }

    public Task<IReadOnlyList<UserDto>?> GetAllAsync()
        => _api.GetAsync<IReadOnlyList<UserDto>>("api/User");

    public Task<UserDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<UserDto>($"api/User/{id}");

    public Task<UserDto?> GetByEmailAsync(string email)
        => _api.GetAsync<UserDto>($"api/User/email/{email}");

    public async Task<bool> AssignRoleAsync(Guid userId, Guid roleId)
    {
        var res = await _api.PostAsync($"api/User/{userId}/roles/{roleId}", null);
        return res;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId)
    {
        var res = await _api.PostAsync($"api/User/{userId}/roles/{roleId}", null);
        return res;
    }

    public async Task<bool> HasRoleAsync(Guid userId, string roleName)
    {
        var result = await _api.GetAsync<RoleCheckResponse>(
            $"api/User/{userId}/roles/check?roleName={roleName}"
        );

        return result?.HasRole ?? false;
    }

    public async Task<bool> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto)
    {
        var res = await _api.PutAsync($"api/User/{userId}/password", dto);
        return res;
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        return await _api.DeleteAsync($"api/User/{userId}");
    }

    public class RoleCheckResponse
    {
        public bool HasRole { get; set; }
    }
}