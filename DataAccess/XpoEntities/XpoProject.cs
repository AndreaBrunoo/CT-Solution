using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

[Persistent("Project")]
public class XpoProject : XPBaseObject
{
    public XpoProject(Session session) : base(session) { }

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

    private decimal hourlyRate;

    [Persistent]
    public decimal HourlyRate
    {
        get => hourlyRate;
        set => SetPropertyValue(nameof(HourlyRate), ref hourlyRate, value);
    }

    // -----------------------------
    // Foreign Keys (Guid)
    // -----------------------------

    private Guid idCompany;

    [Persistent]
    public Guid IdCompany
    {
        get => idCompany;
        set => SetPropertyValue(nameof(IdCompany), ref idCompany, value);
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

    [Association("Company-Projects")]
    public XpoCompany? Company
    {
        get => company;
        set => SetPropertyValue(nameof(Company), ref company, value);
    }
    private XpoCompany? company;

    // -----------------------------
    // Relations
    // -----------------------------

    [Association("Project-WorkLogs")]
    public XPCollection<XpoWorkLog> WorkLogs => GetCollection<XpoWorkLog>(nameof(WorkLogs));
}