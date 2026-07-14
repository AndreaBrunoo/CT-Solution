using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IStatusService
{
    Task<StatusDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<StatusDto>?> GetAllAsync(CancellationToken ct = default);
    Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken ct = default);
    Task<StatusDto?> UpdateAsync(UpdateStatusDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}