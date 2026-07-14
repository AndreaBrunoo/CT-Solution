namespace Sln.Domain.Entities;

public class WorkLog
{
    public WorkLog(
        Guid id,
        string name,
        string description,
        int hoursCounter,
        DateOnly date,
        DateTime createdAt,
        DateTime updatedAt,
        Guid idProject,
        Guid idEmployee,
        Guid idCategory,
        Guid idStatus)
    {
        Id = id;
        Name = name;
        Description = description;
        HoursCounter = hoursCounter;
        Date = date;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IdProject = idProject;
        IdEmployee = idEmployee;
        IdCategory = idCategory;
        IdStatus = idStatus;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HoursCounter { get; set; }
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid IdProject { get; set; }
    public Project? Project { get; set; }

    public Guid IdEmployee { get; set; }
    public Employee? Employee { get; set; }

    public Guid IdCategory { get; set; }
    public Category? Category { get; set; }

    public Guid IdStatus { get; set; }
    public Status? Status { get; set; }
}