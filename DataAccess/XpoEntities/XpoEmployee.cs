using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoEmployee : XPBaseObject
{
    public XpoEmployee(Session session) : base(session) { }

    // -----------------------------
    // Primary Key (GUID)
    // -----------------------------

    [Key(true)]
    public Guid Id { get; set; }

    // -----------------------------
    // Primitive Fields
    // -----------------------------

    private string userName = string.Empty;
    public string UserName
    {
        get => userName;
        set => SetPropertyValue(nameof(UserName), ref userName, value);
    }

    // -----------------------------
    // Navigation Properties
    // -----------------------------

    [Association("User-Employees")]
    public XpoUser User
    {
        get => user;
        set => SetPropertyValue(nameof(User), ref user, value);
    }
    private XpoUser user;

    // -----------------------------
    // Relations
    // -----------------------------

    [Association("Employee-WorkLogs")]
    public XPCollection<XpoWorkLog> WorkLogs => GetCollection<XpoWorkLog>(nameof(WorkLogs));
}