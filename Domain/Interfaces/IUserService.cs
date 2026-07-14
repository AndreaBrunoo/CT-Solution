using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;
public interface IUserService
{
    Task RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct);
    Task<IReadOnlyList<UserDto>?> GetAllAsync(CancellationToken ct);
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct);
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct);
}