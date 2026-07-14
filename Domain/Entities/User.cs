namespace Sln.Domain.Entities;

public class User
{
    public User(
        Guid id,
        string email,
        string passwordHash,
        string passwordSalt,
        ICollection<Role> roles)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Roles = roles;
    }

    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    
    public Employee? Employee { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();
}