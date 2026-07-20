using Frontend.Domain.Dtos.User;

namespace Frontend.Domain.Contracts;

public interface IUserService
{
    Task<UserDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<CurrentUserDto?> GetCurrentUserAsync();
    Task<IReadOnlyList<UserDto>?> GetAllAsync();
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
    Task<bool> HasRoleAsync(Guid userId, string roleName);
    Task<bool> UpdatePasswordAsync(Guid userId, UpdatePasswordDto dto);
    Task<bool> DeleteAsync(Guid userId);
}