namespace Sln.Domain.Entities;

public class User
{
    public User(
        Guid id,
        string email,
        string passwordHash,
        string passwordSalt,
        string role)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
    }

    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    public Employee? Employee { get; set; }
}