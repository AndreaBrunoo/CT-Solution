using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ProjectDto>?> GetAllAsync(CancellationToken ct = default);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken ct = default);
    Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}