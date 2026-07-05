using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid IdCompany { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? CompanyName { get; set; }
}

public class CreateProjectDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal HourlyRate { get; set; }

    [Required]
    public Guid IdCompany { get; set; }
}

public class UpdateProjectDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal HourlyRate { get; set; }

    [Required]
    public Guid IdCompany { get; set; }
}