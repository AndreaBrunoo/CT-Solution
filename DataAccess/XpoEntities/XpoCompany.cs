using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoCompany : XPBaseObject
{
    public XpoCompany(Session session) : base(session) { }

    // -----------------------------
    // Primary Key (GUID)
    // -----------------------------

    [Key(true)]
    public Guid Id { get; set; }

    // -----------------------------
    // Primitive Fields
    // -----------------------------

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    private string email = string.Empty;
    public string Email
    {
        get => email;
        set => SetPropertyValue(nameof(Email), ref email, value);
    }

    private decimal hourlyRate;
    public decimal HourlyRate
    {
        get => hourlyRate;
        set => SetPropertyValue(nameof(HourlyRate), ref hourlyRate, value);
    }

    // -----------------------------
    // Relations
    // -----------------------------

    [Association("Company-Projects")]
    public XPCollection<XpoProject> Projects => GetCollection<XpoProject>(nameof(Projects));
}