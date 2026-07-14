using System.ComponentModel.DataAnnotations;
using Sln.Domain.Entities;

namespace Sln.Domain.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class CreatePermissionDto
{
    [Required]
    public string Code { get; set; } = string.Empty;
}

public class UpdatePermissionDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;
}