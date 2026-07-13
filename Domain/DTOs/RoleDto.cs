namespace Sln.Domain.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public IReadOnlyList<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
}