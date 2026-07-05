using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IWorkLogService
{
    Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkLogDto>?> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto, CancellationToken cancellationToken = default);
    Task<WorkLogDto?> UpdateAsync(UpdateWorkLogDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}