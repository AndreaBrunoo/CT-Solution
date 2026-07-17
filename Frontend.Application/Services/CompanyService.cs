using Frontend.Domain.Dtos.Company;
using Frontend.Infrastructure.Api;
using Frontend.Domain.Contracts;

namespace Frontend.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IApiClient _api;

    public CompanyService(IApiClient api)
    {
        _api = api;
    }

    public Task<CompanyDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<CompanyDto>($"api/Company/{id}");

    public  async Task<IReadOnlyList<CompanyDto>?> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<CompanyDto>>("api/Company");
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        var result = await _api.PostAsync<CompanyDto>("api/Company/create", dto);
        return result!;
    }

    public async Task<CompanyDto?> UpdateAsync(UpdateCompanyDto dto)
    {
        return await _api.PutAsync<CompanyDto>("api/Company/update", dto);
         
    }

    public Task<bool> DeleteAsync(Guid id)
        => _api.DeleteAsync($"api/Company/delete?id={id}");
}