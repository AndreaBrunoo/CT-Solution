using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoWorkLog : XPBaseObject
{
    public XpoWorkLog(Session session) : base(session) { }

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

    private string description = string.Empty;

    [Persistent]
    public string Description
    {
        get => description;
        set => SetPropertyValue(nameof(Description), ref description, value);
    }

    private int hoursCounter;

    [Persistent]
    public int HoursCounter
    {
        get => hoursCounter;
        set => SetPropertyValue(nameof(HoursCounter), ref hoursCounter, value);
    }

    private DateOnly date;

    [Persistent]
    public DateOnly Date
    {
        get => date;
        set => SetPropertyValue(nameof(Date), ref date, value);
    }

    private DateTime createdAt;

    [Persistent]
    public DateTime CreatedAt
    {
        get => createdAt;
        set => SetPropertyValue(nameof(CreatedAt), ref createdAt, value);
    }

    private DateTime updatedAt;

    [Persistent]
    public DateTime UpdatedAt
    {
        get => updatedAt;
        set => SetPropertyValue(nameof(UpdatedAt), ref updatedAt, value);
    }

    // -----------------------------
    // Foreign Keys (Guid)
    // -----------------------------

    private Guid idProject;

    [Persistent]
    public Guid IdProject
    {
        get => idProject;
        set => SetPropertyValue(nameof(IdProject), ref idProject, value);
    }

    private Guid idEmployee;

    [Persistent]
    public Guid IdEmployee
    {
        get => idEmployee;
        set => SetPropertyValue(nameof(IdEmployee), ref idEmployee, value);
    }

    private Guid idCategory;

    [Persistent]
    public Guid IdCategory
    {
        get => idCategory;
        set => SetPropertyValue(nameof(IdCategory), ref idCategory, value);
    }

    private Guid idStatus;

    [Persistent]
    public Guid IdStatus
    {
        get => idStatus;
        set => SetPropertyValue(nameof(IdStatus), ref idStatus, value);
    }

    // -----------------------------
    // Navigation Properties
    // -----------------------------

    [Association("Project-WorkLogs")]
    public XpoProject Project
    {
        get => project;
        set => SetPropertyValue(nameof(Project), ref project, value);
    }
    private XpoProject project;

    [Association("Employee-WorkLogs")]
    public XpoEmployee Employee
    {
        get => employee;
        set => SetPropertyValue(nameof(Employee), ref employee, value);
    }
    private XpoEmployee employee;

    [Association("Category-WorkLogs")]
    public XpoCategory Category
    {
        get => category;
        set => SetPropertyValue(nameof(Category), ref category, value);
    }
    private XpoCategory category;

    [Association("Status-WorkLogs")]
    public XpoStatus Status
    {
        get => status;
        set => SetPropertyValue(nameof(Status), ref status, value);
    }
    private XpoStatus status;
}