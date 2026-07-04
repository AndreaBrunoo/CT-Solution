using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IStatusService
{
    Task<StatusDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StatusDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken cancellationToken = default);
    Task<StatusDto?> UpdateAsync(UpdateStatusDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}