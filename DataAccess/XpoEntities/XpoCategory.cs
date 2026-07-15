using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

[Persistent("Category")]
public class XpoCategory : XPBaseObject
{
    public XpoCategory(Session session) : base(session) { }

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

    // -----------------------------
    // Relations
    // -----------------------------

    [Association("Category-WorkLogs")]
    public XPCollection<XpoWorkLog> WorkLogs => GetCollection<XpoWorkLog>(nameof(WorkLogs));
}