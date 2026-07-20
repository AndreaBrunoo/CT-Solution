using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.DTOs;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class UpdateEmployeeDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;
}