using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class StatusService : IStatusService
{
    private readonly XpoDataContext _ctx;
    private readonly IActionLogger _logger;

    public StatusService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    public async Task<StatusDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(id, ct);
            if (xpo == null || xpo.IsDeleted) return null;

            var domain = XpoStatusMapper.ToDomain(xpo);

            return XpoStatusMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<StatusDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoStatus>()
                .Where(x => !x.IsDeleted)
                .ToListAsync(ct);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoStatusMapper.ToDomain(xpo);

                return XpoStatusMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoStatus>()
                .FirstOrDefaultAsync(w =>
                    !w.IsDeleted && w.Name == dto.Name,
                    ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Status", null,
                    $"Status with name '{dto.Name}' already exists", ct);
                throw new Exception("Status already exists");
            }

            // Domain
            var domain = new Status(
                id: Guid.NewGuid(),
                name: dto.Name
            );

            // XPO
            var xpo = XpoStatusMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Status", domain.Id, ct);

            // Output DTO
            return XpoStatusMapper.ToDto(domain);
        });
    }

    public async Task<StatusDto?> UpdateAsync(UpdateStatusDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // 1. Carico l'XPO esistente tramite ID
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(dto.Id, ct);
            if (xpo == null || xpo.IsDeleted)
            {
                await _logger.LogFailureAsync("Update", "Status", dto.Id,
                    "Status not found", ct);
                throw new Exception("Status not found");
            }

            // 2. Converto XPO → Domain
            var domain = XpoStatusMapper.ToDomain(xpo);

            // 3. Aggiorno il Domain con i valori del DTO
            domain.Name = dto.Name;

            // 4. Aggiorno l'XPO tramite il mapper
            XpoStatusMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Status", domain.Id, ct);

            // 5. Restituisco il DTO di output
            return XpoStatusMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("SoftDelete", "Status", id,
                    "Status not found", ct);
                throw new Exception("Status not found");
            }

            if (!xpo.IsDeleted)
            {
                xpo.IsDeleted = true;
                xpo.DeletedAt = DateTime.UtcNow;
            }

            await _logger.LogSuccessAsync(uow, "SoftDelete", "Status", id, ct);

            return true;
        });
    }

    public async Task RestoreAsync(Guid id, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Restore", "Status", id,
                    "Status not found", ct);
                throw new Exception("Status not found");
            }

            xpo.IsDeleted = false;
            xpo.DeletedAt = null;

            await _logger.LogSuccessAsync(uow, "Restore", "Status", id, ct);

            return true;
        });
    }
}
