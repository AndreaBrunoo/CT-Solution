using Frontend.Dtos.Role;
namespace Frontend.Dtos.User;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<RoleDto> Roles { get; set; } = new List<RoleDto>();
}
