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

    [Association("User-Employees")]
    public XPCollection<XpoEmployee> Employees => GetCollection<XpoEmployee>(nameof(Employees));

    [Association("User-Roles")]
    public XPCollection<XpoRole> Roles => GetCollection<XpoRole>(nameof(Roles));
}