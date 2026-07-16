namespace Frontend.Dtos.WorkLog;

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
