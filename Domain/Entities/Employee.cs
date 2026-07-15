namespace Sln.Domain.Entities;

public class Employee
{
    public Employee(
        Guid id,
        string userName)
    {
        Id = id;
        UserName = userName;
    }

    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    
    public Guid? IdUser { get; set; }
    public User? User { get; set; }
    
    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}