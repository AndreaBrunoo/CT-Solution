using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.DTOs;

public class StatusDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class CreateStatusDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateStatusDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}