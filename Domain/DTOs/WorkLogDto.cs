using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.DTOs;

public class WorkLogDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HoursCounter { get; set; }
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid IdProject { get; set; }
    public Guid IdEmployee { get; set; }
    public Guid IdCategory { get; set; }
    public Guid IdStatus { get; set; }
    public string? ProjectName { get; set; }
    public string? EmployeeName { get; set; }
    public string? CategoryName { get; set; }
    public string? StatusName { get; set; }
}

public class CreateWorkLogDto
{
    [Required]
    [StringLength(1000)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int HoursCounter { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public Guid IdProject { get; set; }

    [Required]
    public Guid IdEmployee { get; set; }

    [Required]
    public Guid IdCategory { get; set; }

    [Required]
    public Guid IdStatus { get; set; }
}

public class UpdateWorkLogDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int HoursCounter { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public Guid IdProject { get; set; }

    [Required]
    public Guid IdEmployee { get; set; }

    [Required]
    public Guid IdCategory { get; set; }

    [Required]
    public Guid IdStatus { get; set; }
}