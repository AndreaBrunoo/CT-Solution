using Frontend.Domain.Dtos.Project;

namespace Frontend.Domain.Contracts;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ProjectDto>?> GetAllAsync();
    Task<ProjectDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto);
    Task<bool> DeleteAsync(Guid id);
}