namespace Sln.Domain.Entities;

public class Company
{
    public Company(
        Guid id,
        string name, 
        string email)
    {
       Id = id;
       Name = name;
       Email = email;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}