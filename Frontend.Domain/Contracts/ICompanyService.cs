using Frontend.Domain.Dtos.Company;

namespace Frontend.Domain.Contracts;

public interface ICompanyService
{
    Task<CompanyDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CompanyDto>?> GetAllAsync();
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
    Task<CompanyDto?> UpdateAsync(UpdateCompanyDto dto);
    Task<bool> DeleteAsync(Guid id);
}