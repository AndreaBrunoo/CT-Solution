using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
}

public class CreateCompanyDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal HourlyRate { get; set; }
}

public class UpdateCompanyDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal HourlyRate { get; set; }
}