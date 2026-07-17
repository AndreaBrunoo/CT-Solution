namespace Frontend.Domain.Dtos.Project;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public decimal? HourlyRate { get; set; }
    public Guid IdCompany { get; set; }
}
