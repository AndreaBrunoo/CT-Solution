using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class ProjectService : IProjectService
{
    private readonly XpoDataContext _ctx;

    public ProjectService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoProject>(id, cancellationToken);
            if (xpo == null) return null;

            var domain = XpoProjectMapper.ToDomain(xpo);

            return XpoProjectMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<ProjectDto>?> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoProject>().ToListAsync(cancellationToken);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoProjectMapper.ToDomain(xpo);

                return XpoProjectMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoProject>()
                .FirstOrDefaultAsync(w =>
                    w.Name == dto.Name &&
                    w.HourlyRate == dto.HourlyRate &&
                    w.Company.Id == dto.IdCompany,
                    cancellationToken);

            if (existing != null)
                throw new Exception("Project already exists");

            // Domain
            var domain = new Project(
                id: Guid.NewGuid(),
                name: dto.Name,
                hourlyRate: dto.HourlyRate,
                idCompany: dto.IdCompany
            );

            // XPO
            var xpo = XpoProjectMapper.ToXpo(domain, uow);

            // Output DTO
            return XpoProjectMapper.ToDto(domain);
        });
    }

    public async Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // 1. Carico l'XPO esistente tramite ID
            var xpo = await uow.GetObjectByKeyAsync<XpoProject>(dto.Id, cancellationToken);
            if (xpo == null)
                throw new Exception("Project not found");

            // 2. Converto XPO → Domain
            var domain = XpoProjectMapper.ToDomain(xpo);

            // 3. Aggiorno il Domain con i valori del DTO
            domain.Name = dto.Name;
            domain.HourlyRate = dto.HourlyRate;
            domain.IdCompany = dto.IdCompany;

            // 4. Aggiorno l’XPO tramite il mapper
            XpoProjectMapper.ToXpo(domain, uow);

            // 5. Restituisco il DTO di output
            return XpoProjectMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoProject>(id, cancellationToken);
            if (xpo == null)
                throw new Exception("Project not found");

            xpo.Delete();

            return true;
        });
    }
}