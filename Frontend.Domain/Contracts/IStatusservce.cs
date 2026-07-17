using Frontend.Domain.Dtos.Status;

namespace Frontend.Domain.Contracts;

public interface IStatusService
{
    Task<StatusDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<StatusDto>?> GetAllAsync();
    Task<StatusDto> CreateAsync(CreateStatusDto dto);
    Task<StatusDto?> UpdateAsync(UpdateStatusDto dto);
    Task<bool> DeleteAsync(Guid id);
}