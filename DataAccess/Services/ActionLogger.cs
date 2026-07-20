using DevExpress.Xpo;
using Microsoft.AspNetCore.Http;
using Sln.Domain.DTOs;
using Sln.Domain.Interfaces;
using Sln.DataAccess.Mappers;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Services;

/// <summary>
/// Logger applicativo che scrive su tabella XpoLog.
/// - Per le azioni di scrittura riuscite: log nella stessa UnitOfWork
///   (commit atomico con l'azione).
/// - Per i fallimenti: log su una UnitOfWork separata per non perdere
///   la traccia in caso di rollback della transazione principale.
/// </summary>
public class ActionLogger : IActionLogger
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UnitOfWork _uow;

    public ActionLogger(IHttpContextAccessor httpContextAccessor, UnitOfWork uow)
    {
        _httpContextAccessor = httpContextAccessor;
        _uow = uow;
    }

    public async Task<LogDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var log = await _uow.GetObjectByKeyAsync<XpoLog>(id, ct);
        return log is null ? null : XpoLogMapper.ToDto(log);
    }

    public (Guid? UserId, string? Email) GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return (null, null);

        var sub = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
               ?? user.FindFirst("sub")?.Value;
        var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return (
            Guid.TryParse(sub, out var id) ? id : (Guid?)null,
            string.IsNullOrEmpty(email) ? null : email
        );
    }

    public Task LogSuccessAsync(
        UnitOfWork uow,
        string action,
        string entity,
        Guid? entityId,
        CancellationToken ct = default)
    {
        var (userId, email) = GetCurrentUser();
        InsertLog(uow, action, entity, entityId, userId, email, success: true, error: null);
        return Task.CompletedTask;
    }

    public Task LogFailureAsync(
        string action,
        string entity,
        Guid? entityId,
        string errorMessage,
        CancellationToken ct = default)
    {
        // UnitOfWork separata: il fallimento dell'azione non deve bloccare il log
        try
        {
            using var failureUow = new UnitOfWork(XpoDefault.DataLayer);
            var (userId, email) = GetCurrentUser();
            InsertLog(failureUow, action, entity, entityId, userId, email,
                success: false, error: errorMessage);
            failureUow.CommitChanges();
        }
        catch
        {
            // Se anche il log fallisce non propaghiamo: il chiamante ha già
            // la sua eccezione di business da gestire.
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<LogDto>> GetAllAsync(CancellationToken ct = default)
    {
        var query = _uow.Query<XpoLog>()
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(ct);

        return query.ContinueWith(t =>
            (IReadOnlyList<LogDto>)t.Result
                .Select(XpoLogMapper.ToDto)
                .ToList());
    }

    private static void InsertLog(
        UnitOfWork uow,
        string action,
        string entity,
        Guid? entityId,
        Guid? userId,
        string? userEmail,
        bool success,
        string? error)
    {
        _ = new XpoLog(uow)
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            UserId = userId,
            UserEmail = userEmail,
            Success = success,
            ErrorMessage = error
        };
    }
}