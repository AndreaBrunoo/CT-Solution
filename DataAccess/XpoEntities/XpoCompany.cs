using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

[Persistent("Company")]
public class XpoCompany : XPBaseObject
{
    public XpoCompany(Session session) : base(session) { }

    // -----------------------------
    // Primary Key (GUID)
    // -----------------------------

    [Key(true)]
    [Persistent]
    public Guid Id { get; set; }

    // -----------------------------
    // Primitive Fields
    // -----------------------------

    private string name = string.Empty;

    [Persistent]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    private string email = string.Empty;

    [Persistent]
    public string Email
    {
        get => email;
        set => SetPropertyValue(nameof(Email), ref email, value);
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
    // Relations
    // -----------------------------

    [Association("Company-Projects")]
    public XPCollection<XpoProject> Projects => GetCollection<XpoProject>(nameof(Projects));
}