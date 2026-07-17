using Frontend.Domain.Dtos.WorkLog;

namespace Frontend.Domain.Contracts;

public interface IWorkLogService
{
    Task<WorkLogDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<WorkLogDto>> GetMineAsync();
    Task<IReadOnlyList<WorkLogDto>?> GetAllAsync();
    Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto);
    Task<WorkLogDto?> UpdateAsync(UpdateWorkLogDto dto);
    Task<WorkLogDto> ChangeStatusAsync(Guid worklogId, Guid newStatusId);
    Task<bool> DeleteAsync(Guid id);
}