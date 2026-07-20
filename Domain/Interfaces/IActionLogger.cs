using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IActionLogger
{
    (Guid? UserId, string? Email) GetCurrentUser();

    Task<IReadOnlyList<LogDto>> GetAllAsync(CancellationToken ct = default);
    Task<LogDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task LogSuccessAsync(
        DevExpress.Xpo.UnitOfWork uow,
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
}