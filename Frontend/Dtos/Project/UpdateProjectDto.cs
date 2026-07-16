namespace Frontend.Dtos.Project;

public class UpdateProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public Guid IdCompany { get; set; }
}
