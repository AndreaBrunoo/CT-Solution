using Frontend.Domain.Dtos.Log;

namespace Frontend.Domain.Contracts;

public interface IActionLogger
{
    (Guid? UserId, string? Email) GetCurrentUser();

    // Rimosso il parametro UnitOfWork
    Task LogSuccessAsync(
        string action,
        string entity,
        Guid? entityId,
        CancellationToken ct = default);

    Task LogFailureAsync(
        string action,
        string entity,
        Guid? entityId,
        string errorMessage,
        CancellationToken ct = default);

    Task<IReadOnlyList<LogDto>> GetAllAsync(CancellationToken ct = default);
}