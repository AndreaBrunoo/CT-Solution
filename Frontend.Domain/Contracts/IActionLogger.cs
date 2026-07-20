using Frontend.Domain.Dtos.Log;

namespace Frontend.Domain.Contracts;

public interface ILogService
{
    Task<IReadOnlyList<LogDto>> GetAllAsync();
    Task<LogDto?> GetByIdAsync(Guid id);
}