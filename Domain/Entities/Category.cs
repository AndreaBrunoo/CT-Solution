namespace Sln.Domain.Entities;

public class Category
{
    public Category(
    Guid id,
    string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}