using Frontend.Domain.Dtos.Employee;
using Frontend.Infrastructure.Api;
using Frontend.Domain.Contracts;

namespace Frontend.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IApiClient _api;

    public EmployeeService(IApiClient api)
    {
        _api = api;
    }

    public Task<EmployeeDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<EmployeeDto>($"api/Employee/{id}");

    public Task<EmployeeDto?> GetProfileAsync()
        => _api.GetAsync<EmployeeDto>($"api/Employee/me");
        
    public async Task<IReadOnlyList<EmployeeDto>?> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<EmployeeDto>>("api/Employee");
    }

    public async Task<IReadOnlyList<EmployeeDto>?> GetByProjectIdAsync(Guid projectId)
    {
        return await _api.GetAsync<IReadOnlyList<EmployeeDto>>(
            $"api/Employee/by-project/{projectId}"
        );
    }
    
    public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto)
    {
        return await _api.PutAsync<EmployeeDto>("api/Employee/update", dto);
    }

    public Task<bool> DeleteAsync(Guid id)
        => _api.DeleteAsync($"api/Employee/delete?id={id}");
}