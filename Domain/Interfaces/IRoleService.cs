using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;
public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(string name, CancellationToken ct);
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct);
    Task DeleteRoleAsync(Guid roleId, CancellationToken ct);
}