namespace Sln.Domain.Entities;

public class Permission
{
    public Permission(
        Guid id,
        string code)
    {
        Id = id;
        Code = code;
    }

    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}