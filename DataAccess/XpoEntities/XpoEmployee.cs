using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

[Persistent("Employee")]
public class XpoEmployee : XPBaseObject
{
    public XpoEmployee(Session session) : base(session) { }

    // -----------------------------
    // Primary Key (GUID)
    // -----------------------------

    [Key(true)]
    [Persistent]
    public Guid Id { get; set; }

    // -----------------------------
    // Primitive Fields
    // -----------------------------

    private string userName = string.Empty;

    [Persistent]
    public string UserName
    {
        get => userName;
        set => SetPropertyValue(nameof(UserName), ref userName, value);
    }

    // -----------------------------
    // Soft Delete
    // -----------------------------

    [NonPersistent]
    public bool IsDeleted
    {
        get => DeletedAt != null;
        set
        {
            if (value)
            {
                if (DeletedAt == null)
                    DeletedAt = DateTime.UtcNow;
            }
            else
            {
                DeletedAt = null;
            }
        }
    }

    private DateTime? deletedAt;

    [Persistent]
    public DateTime? DeletedAt
    {
        get => deletedAt;
        set => SetPropertyValue(nameof(DeletedAt), ref deletedAt, value);
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