using Frontend.Domain.Dtos.Role;
using Frontend.Infrastructure.Api;
using Frontend.Domain.Contracts;

namespace Frontend.Application.Services;

public class RoleService : IRoleService
{
    private readonly IApiClient _api;

    public RoleService(IApiClient api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<RoleDto>>("api/Role");
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        var result = await _api.PostAsync<RoleDto>("api/Role/create", dto);
        return result;
    }

    public Task<bool> DeleteRoleAsync(Guid roleId)
        => _api.DeleteAsync($"api/Role?roleId={roleId}");
}