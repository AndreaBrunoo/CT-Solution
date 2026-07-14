using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoLog : XPBaseObject
{
    public XpoLog(Session session) : base(session) { }

    [Key(true)]
    public Guid Id { get; set; }

    private DateTime timestamp;
    public DateTime Timestamp
    {
        get => timestamp;
        set => SetPropertyValue(nameof(Timestamp), ref timestamp, value);
    }

    private string action = string.Empty;
    /// <summary>Es.: "Create", "Update", "Delete", "AssignRole", "Login".</summary>
    public string Action
    {
        get => action;
        set => SetPropertyValue(nameof(Action), ref action, value);
    }

    private string entity = string.Empty;
    /// <summary>Es.: "WorkLog", "Project", "User".</summary>
    public string Entity
    {
        get => entity;
        set => SetPropertyValue(nameof(Entity), ref entity, value);
    }

    private Guid? entityId;
    /// <summary>Id dell'entità toccata (null per azioni globali come Login/Register).</summary>
    public Guid? EntityId
    {
        get => entityId;
        set => SetPropertyValue(nameof(EntityId), ref entityId, value);
    }

    private Guid? userId;
    /// <summary>Id dell'utente che ha eseguito l'azione (null per Login fallito o endpoint pubblici).</summary>
    public Guid? UserId
    {
        get => userId;
        set => SetPropertyValue(nameof(UserId), ref userId, value);
    }

    private string? userEmail;
    public string? UserEmail
    {
        get => userEmail;
        set => SetPropertyValue(nameof(UserEmail), ref userEmail, value);
    }

    private bool success;
    public bool Success
    {
        get => success;
        set => SetPropertyValue(nameof(Success), ref success, value);
    }

    private string? errorMessage;
    /// <summary>Popolato solo se Success = false.</summary>
    public string? ErrorMessage
    {
        get => errorMessage;
        set => SetPropertyValue(nameof(ErrorMessage), ref errorMessage, value);
    }
}
