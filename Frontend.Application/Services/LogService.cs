using Frontend.Domain.Contracts;
using Frontend.Domain.Dtos.Log;
using Frontend.Infrastructure.Api;

public class LogService : ILogService
{
    private readonly IApiClient _api;

    public LogService(IApiClient api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<LogDto>> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<LogDto>>("api/Log");
    }

    public async Task<LogDto?> GetByIdAsync(Guid id)
    {
        return await _api.GetAsync<LogDto>($"api/Log/{id}");
    }
}
