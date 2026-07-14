using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoPermission : XPBaseObject
{
    public XpoPermission(Session session) : base(session) { }

    [Key(true)]
    [Persistent]
    public Guid Id { get; set; }

    private string code = string.Empty;

    [Persistent]
    public string Code
    {
        get => code;
        set => SetPropertyValue(nameof(Code), ref code, value);
    }

    [Association("Role-Permissions")]
    public XPCollection<XpoRole> Roles => GetCollection<XpoRole>(nameof(Roles));
}