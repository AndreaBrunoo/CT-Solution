namespace Sln.Domain.Entities;

public class Status
{
    public Status(
        Guid id,
        string name)
    {
        Id = id;
        Name = name;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}