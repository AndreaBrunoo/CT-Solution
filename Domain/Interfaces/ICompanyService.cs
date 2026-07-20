using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CompanyDto>?> GetAllAsync(CancellationToken ct = default);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto, CancellationToken ct = default);
    Task<CompanyDto?> UpdateAsync(UpdateCompanyDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task RestoreAsync(Guid id, CancellationToken ct = default);
}