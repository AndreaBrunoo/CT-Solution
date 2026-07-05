using DevExpress.Xpo;

namespace Sln.DataAccess.XpoEntities;

public class XpoWorkLog : XPBaseObject
{
    public XpoWorkLog(Session session) : base(session) { }

    // -----------------------------
    // Primary Key (GUID)
    // -----------------------------

    [Key(true)]
    public Guid Id { get; set; }

    // -----------------------------
    // Primitive Fields
    // -----------------------------

    private string description = string.Empty;
    public string Description 
    {
        get => description;
        set => SetPropertyValue(nameof(Description), ref description, value);
    }

    private int hoursCounter;
    public int HoursCounter
    {
        get => hoursCounter;
        set => SetPropertyValue(nameof(HoursCounter), ref hoursCounter, value);
    }

    private DateOnly date;
    public DateOnly Date
    {
        get => date;
        set => SetPropertyValue(nameof(Date), ref date, value);
    }

    private DateTime createdAt;
    public DateTime CreatedAt
    {
        get => createdAt;
        set => SetPropertyValue(nameof(CreatedAt), ref createdAt, value);
    }

    private DateTime updatedAt;
    public DateTime UpdatedAt
    {
        get => updatedAt;
        set => SetPropertyValue(nameof(UpdatedAt), ref updatedAt, value);
    }

    // -----------------------------
    // Foreign Keys (Guid)
    // -----------------------------

    private Guid idProject;
    public Guid IdProject
    {
        get => idProject;
        set => SetPropertyValue(nameof(IdProject), ref idProject, value);
    }

    private Guid idEmployee;
    public Guid IdEmployee
    {
        get => idEmployee;
        set => SetPropertyValue(nameof(IdEmployee), ref idEmployee, value);
    }

    private Guid idCategory;
    public Guid IdCategory
    {
        get => idCategory;
        set => SetPropertyValue(nameof(IdCategory), ref idCategory, value);
    }

    private Guid idStatus;
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