using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IWorkLogService
{
    Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<WorkLogDto>> GetMineAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<WorkLogDto>?> GetAllAsync(CancellationToken ct = default);
    Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto, CancellationToken ct = default);
    Task<WorkLogDto?> UpdateAsync(UpdateWorkLogDto dto, CancellationToken ct = default);
    Task<WorkLogDto> ChangeStatusAsync(Guid worklogId, Guid newStatusId, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}