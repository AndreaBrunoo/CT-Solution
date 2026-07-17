using Frontend.Domain.Dtos.WorkLog;
using Frontend.Infrastructure.Api;
using Frontend.Domain.Contracts;

namespace Frontend.Application.Services;

public class WorkLogService : IWorkLogService
{

  private readonly IApiClient _api;

    public WorkLogService(IApiClient api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<WorkLogDto>> GetMineAsync()
    {
        return await _api.GetAsync<IReadOnlyList<WorkLogDto>>("api/WorkLog/mine");
    }

    public async Task<IReadOnlyList<WorkLogDto>?> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<WorkLogDto>>("api/WorkLog");
    }

    public Task<WorkLogDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<WorkLogDto>($"api/WorkLog/{id}");

    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto)
    {
        var result = await _api.PostAsync<WorkLogDto>("api/WorkLog/create", dto);
        return result;
    }

    public async Task<WorkLogDto?> UpdateAsync(UpdateWorkLogDto dto)
    {
        return await _api.PutAsync<WorkLogDto>("api/WorkLog/update", dto);
    }

    public  Task<bool> DeleteAsync(Guid id)
        => _api.DeleteAsync($"api/WorkLog/delete?id={id}");

    public async Task<WorkLogDto> ChangeStatusAsync(Guid worklogId, Guid newStatusId)
    {
        var result = await _api.PostAsync<WorkLogDto>(
            $"api/WorkLog/{worklogId}/change-status?newStatusId={newStatusId}",
            null
        );
        return result;
    }
}