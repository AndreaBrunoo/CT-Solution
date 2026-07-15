using System.ComponentModel.DataAnnotations;
using Sln.Domain.Entities;

namespace Sln.Domain.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateRoleDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}