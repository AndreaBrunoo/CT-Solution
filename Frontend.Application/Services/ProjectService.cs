using Frontend.Domain.Dtos.Project;
using Frontend.Infrastructure.Api;
using Frontend.Domain.Contracts;

namespace Frontend.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IApiClient _api;

    public ProjectService(IApiClient api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<ProjectDto>?> GetAllAsync()
    {
        return await _api.GetAsync<IReadOnlyList<ProjectDto>>("api/Project");
    }

    public Task<ProjectDto?> GetByIdAsync(Guid id)
        => _api.GetAsync<ProjectDto>($"api/Project/{id}");

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
    {
        var result = await _api.PostAsync<ProjectDto>("api/Project/create", dto);
        return result;
    }

    public async Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto)
    {
        return await _api.PutAsync<ProjectDto>("api/Project/update", dto);
    }

    public Task<bool> DeleteAsync(Guid id)
        => _api.DeleteAsync($"api/Project/delete?id={id}");
}