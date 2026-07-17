namespace Frontend.Domain.Dtos.Project;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid IdCompany { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? CompanyName { get; set; }
}
