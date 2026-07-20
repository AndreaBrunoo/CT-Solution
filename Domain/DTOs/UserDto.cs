using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<RoleDto> Roles { get; set; } = new List<RoleDto>();
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class CurrentUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}

public class UpdatePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string Email { get; set; } = string.Empty;
}