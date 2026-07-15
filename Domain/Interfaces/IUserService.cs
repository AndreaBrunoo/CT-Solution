using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IUserService
{
    Task RegisterAsync(RegisterDto dto, CancellationToken ct);
    Task<string> LoginAsync(LoginDto dto, CancellationToken ct);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct);
    Task<IReadOnlyList<UserDto>?> GetAllAsync(CancellationToken ct);
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
    Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct);
    Task UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto, CancellationToken ct);
    Task DeleteAsync(Guid userId, CancellationToken ct);
}