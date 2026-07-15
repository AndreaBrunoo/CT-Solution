using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoRole : XPBaseObject
{
    public XpoRole(Session session) : base(session) { }

    [Key(true)]
    [Persistent]
    public Guid Id { get; set; }

    private string name = string.Empty;
    
    [Persistent]
    public string Name
    {
        get => name;
        set => SetPropertyValue(nameof(Name), ref name, value);
    }

    [Association("User-Roles")]
    public XPCollection<XpoUser> Users => GetCollection<XpoUser>(nameof(Users));
}