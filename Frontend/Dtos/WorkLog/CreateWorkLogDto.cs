namespace Frontend.Dtos.WorkLog;

public class CreateWorkLogDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HoursCounter { get; set; }
    public DateOnly Date { get; set; }
    public Guid IdProject { get; set; }
    public Guid IdEmployee { get; set; }
    public Guid IdCategory { get; set; }
    public Guid IdStatus { get; set; }
}