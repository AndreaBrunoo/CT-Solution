using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoUser : XPBaseObject
{
    public XpoUser(Session session) : base(session) { }

    // -----------------------------
    // Primary Key (GUID)
    // -----------------------------

    [Key(true)]
    public Guid Id { get; set; }

    // -----------------------------
    // Primitive Fields
    // -----------------------------

    private string email = string.Empty;
    public string Email
    {
        get => email;
        set => SetPropertyValue(nameof(Email), ref email, value);
    }

    private string passwordHash = string.Empty;
    public string PasswordHash
    {
        get => passwordHash;
        set => SetPropertyValue(nameof(PasswordHash), ref passwordHash, value);
    }

    private string passwordSalt = string.Empty;
    public string PasswordSalt
    {
        get => passwordSalt;
        set => SetPropertyValue(nameof(PasswordSalt), ref passwordSalt, value);
    }

    private string role = string.Empty;
    public string Role
    {
        get => role;
        set => SetPropertyValue(nameof(Role), ref role, value);
    }

    // -----------------------------
    // Navigation Properties
    // -----------------------------

    [Association("Employee-User")]
    public XpoEmployee Employee
    {
        get => employee;
        set => SetPropertyValue(nameof(Employee), ref employee, value);
    }
    private XpoEmployee employee;
}