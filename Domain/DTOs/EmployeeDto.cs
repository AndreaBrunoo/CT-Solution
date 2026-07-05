using System.ComponentModel.DataAnnotations;
using Sln.Domain.Entities;

namespace Sln.Domain.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class CreateEmployeeDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public Guid IdUser { get; set; }
}

public class UpdateEmployeeDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;
}