using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

[Persistent("Log")]
public class XpoLog : XPBaseObject
{
    public XpoLog(Session session) : base(session) { }

    [Key(true)]
    [Persistent]
    public Guid Id { get; set; }

    private DateTime timestamp;

    [Persistent]
    public DateTime Timestamp
    {
        get => timestamp;
        set => SetPropertyValue(nameof(Timestamp), ref timestamp, value);
    }

    private string action = string.Empty;
    
    [Persistent]
    public string Action
    {
        get => action;
        set => SetPropertyValue(nameof(Action), ref action, value);
    }

    private string entity = string.Empty;

    [Persistent]
    public string Entity
    {
        get => entity;
        set => SetPropertyValue(nameof(Entity), ref entity, value);
    }

    private Guid? entityId;

    [Persistent]
    public Guid? EntityId
    {
        get => entityId;
        set => SetPropertyValue(nameof(EntityId), ref entityId, value);
    }

    private Guid? userId;

    [Persistent]
    public Guid? UserId
    {
        get => userId;
        set => SetPropertyValue(nameof(UserId), ref userId, value);
    }

    private string? userEmail;

    [Persistent]
    public string? UserEmail
    {
        get => userEmail;
        set => SetPropertyValue(nameof(UserEmail), ref userEmail, value);
    }

    private bool success;

    [Persistent]
    public bool Success
    {
        get => success;
        set => SetPropertyValue(nameof(Success), ref success, value);
    }

    private string? errorMessage;

    [Persistent]
    public string? ErrorMessage
    {
        get => errorMessage;
        set => SetPropertyValue(nameof(ErrorMessage), ref errorMessage, value);
    }
}