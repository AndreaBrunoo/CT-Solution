using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

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
    // Navigation Properties
    // -----------------------------

    [Association("Company-Projects")]
    [Persistent]
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