using Frontend.Domain.Dtos.Status;
using Frontend.Infrastructure.Api;
using Frontend.Domain.Contracts;

namespace Frontend.Application.Services;

public class StatusService : IStatusService
{
    private readonly IApiClient _api;

    public StatusService(IApiClient api)
    {
        _api = api;
    }

    public Task<StatusDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<StatusDto>($"api/Status/{id}");

    public async Task<IReadOnlyList<StatusDto>?> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<StatusDto>>("api/Status");
    }

    public async Task<StatusDto> CreateAsync(CreateStatusDto dto)
    {
        var result = await _api.PostAsync<StatusDto>("api/Status/create", dto);
        return result;
    }

    public async Task<StatusDto?> UpdateAsync(UpdateStatusDto dto)
    {
        return await _api.PutAsync<StatusDto>("api/Status/update", dto);
    }

    public async Task<bool> DeleteAsync(Guid id)
        => await _api.DeleteAsync($"api/Status/delete?id={id}");
}