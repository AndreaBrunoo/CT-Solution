using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoStatus : XPBaseObject
{
    public XpoStatus(Session session) : base(session) { }

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

    // -----------------------------
    // Relations
    // -----------------------------

    [Association("Status-WorkLogs")]
    public XPCollection<XpoWorkLog> WorkLogs => GetCollection<XpoWorkLog>(nameof(WorkLogs));
}