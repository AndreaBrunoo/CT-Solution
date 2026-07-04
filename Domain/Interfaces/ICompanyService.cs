using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompanyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default);
    Task<CompanyDto?> UpdateAsync(UpdateCompanyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}