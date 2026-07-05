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

    public StatusService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    public async Task<StatusDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(id, cancellationToken);
            if (xpo == null) return null;

            var domain = XpoStatusMapper.ToDomain(xpo);

            return XpoStatusMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<StatusDto>?> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoStatus>().ToListAsync(cancellationToken);

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

    public async Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoStatus>()
                .FirstOrDefaultAsync(w =>
                    w.Name == dto.Name,
                    cancellationToken);

            if (existing != null)
                throw new Exception("Status already exists");

            // Domain
            var domain = new Status(
                id: Guid.NewGuid(),
                name: dto.Name
            );

            // XPO
            var xpo = XpoStatusMapper.ToXpo(domain, uow);

            // Output DTO
            return XpoStatusMapper.ToDto(domain);
        });
    }

    public async Task<StatusDto?> UpdateAsync(UpdateStatusDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // 1. Carico l'XPO esistente tramite ID
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(dto.Id, cancellationToken);
            if (xpo == null)
                throw new Exception("Status not found");

            // 2. Converto XPO → Domain
            var domain = XpoStatusMapper.ToDomain(xpo);

            // 3. Aggiorno il Domain con i valori del DTO
            domain.Name = dto.Name;

            // 4. Aggiorno l’XPO tramite il mapper
            XpoStatusMapper.ToXpo(domain, uow);

            // 5. Restituisco il DTO di output
            return XpoStatusMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoStatus>(id, cancellationToken);
            if (xpo == null)
                throw new Exception("Status not found");

            xpo.Delete();

            return true;
        });
    }
}