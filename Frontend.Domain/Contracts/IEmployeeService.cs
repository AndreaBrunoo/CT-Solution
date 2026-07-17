using Frontend.Domain.Dtos.Employee;

namespace Frontend.Domain.Contracts;

public interface IEmployeeService
{
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto?> GetProfileAsync();
    Task<IReadOnlyList<EmployeeDto>?> GetAllAsync();
    Task<IReadOnlyList<EmployeeDto>?> GetByProjectIdAsync(Guid projectId);
    Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
}