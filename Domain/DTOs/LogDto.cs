namespace Sln.Domain.DTOs;

public class LogDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}