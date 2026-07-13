using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;
public interface IPermissionService
{
    Task<PermissionDto> CreatePermissionAsync(string code, CancellationToken ct);
    Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken ct);
    Task DeletePermissionAsync(Guid permissionId, CancellationToken ct);
}