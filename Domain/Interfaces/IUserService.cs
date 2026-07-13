using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;
public interface IUserService
{
    Task RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserDto>?> GetAllAsync(CancellationToken cancellationToken);
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct);
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct);
}