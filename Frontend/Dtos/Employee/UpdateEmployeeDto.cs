namespace Frontend.Dtos.Employee;

public class UpdateEmployeeDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}
