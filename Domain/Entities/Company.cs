namespace Sln.Domain.Entities;

public class Company
{
    public Company(
        Guid id,
        string name, 
        string email,
        decimal hourlyRate)
    {
       Id = id;
       Name = name;
       Email = email;
       HourlyRate = hourlyRate; 
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}