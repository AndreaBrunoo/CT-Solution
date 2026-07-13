using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IRolePermissionService
{
    Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct);
    Task RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct);
    Task<bool> HasPermissionAsync(Guid roleId, string permissionCode, CancellationToken ct);
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(Guid roleId, CancellationToken ct);
}