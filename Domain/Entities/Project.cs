namespace Sln.Domain.Entities;

public class Project
{
    public Project(
        Guid id,
        string name,
        decimal hourlyRate,
        Guid idCompany)
    {
        Id = id;
        Name = name;
        HourlyRate = hourlyRate;
        IdCompany = idCompany;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }

    public Guid IdCompany { get; set; }
    public Company? Company { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}