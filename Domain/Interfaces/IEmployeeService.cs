using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<EmployeeDto?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeDto>?> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeDto>?> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task RestoreAsync(Guid id, CancellationToken ct = default);
}