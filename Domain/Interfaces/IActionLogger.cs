using Sln.Domain.DTOs;

namespace Sln.Domain.Interfaces;

public interface IActionLogger
{
    /// <summary>
    /// Restituisce l'utente corrente desunto dal JWT (o null se non autenticato).
    /// </summary>
    (Guid? UserId, string? Email) GetCurrentUser();

    /// <summary>
    /// Log di un'azione andata a buon fine. Da invocare DENTRO la UnitOfWork attiva,
    /// in modo che la riga di log committi insieme all'azione.
    /// </summary>
    Task LogSuccessAsync(
        DevExpress.Xpo.UnitOfWork uow,
        string action,
        string entity,
        Guid? entityId,
        CancellationToken ct = default);

    /// <summary>
    /// Log di un'azione fallita. Da invocare PRIMA di lanciare l'eccezione
    /// (o in un catch) passando un'UnitOfWork separata per non perdere il log
    /// se la transazione principale fa rollback.
    /// </summary>
    Task LogFailureAsync(
        string action,
        string entity,
        Guid? entityId,
        string errorMessage,
        CancellationToken ct = default);

    /// <summary>
    /// Storico log (solo lettura) per future pagine di audit.
    /// </summary>
    Task<IReadOnlyList<LogDto>> GetAllAsync(CancellationToken ct = default);
}