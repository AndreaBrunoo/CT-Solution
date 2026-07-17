using Frontend.Domain.Dtos.Role;

namespace Frontend.Domain.Contracts;

public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
    Task<IReadOnlyList<RoleDto>> GetAllAsync();
    Task<bool> DeleteRoleAsync(Guid roleId);
}