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
    private readonly IActionLogger _logger;

    public ProjectService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoProject>(id, ct);
            if (xpo == null) return null;

            var domain = XpoProjectMapper.ToDomain(xpo);

            return XpoProjectMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<ProjectDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoProject>().ToListAsync(ct);

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

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoProject>()
                .FirstOrDefaultAsync(w =>
                    w.Name == dto.Name &&
                    w.HourlyRate == dto.HourlyRate &&
                    w.Company.Id == dto.IdCompany,
                    ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Project", null,
                    $"Project '{dto.Name}' for company {dto.IdCompany} already exists",
                    ct);
                throw new Exception("Project already exists");
            }

            var domain = new Project(
                id: Guid.NewGuid(),
                name: dto.Name,
                hourlyRate: dto.HourlyRate,
                idCompany: dto.IdCompany
            );

            var xpo = XpoProjectMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Project", domain.Id, ct);

            return XpoProjectMapper.ToDto(domain);
        });
    }

    public async Task<ProjectDto?> UpdateAsync(UpdateProjectDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoProject>(dto.Id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Update", "Project", dto.Id,
                    "Project not found", ct);
                throw new Exception("Project not found");
            }

            var domain = XpoProjectMapper.ToDomain(xpo);

            domain.Name = dto.Name;
            domain.HourlyRate = dto.HourlyRate;
            domain.IdCompany = dto.IdCompany;

            XpoProjectMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Project", domain.Id, ct);

            return XpoProjectMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoProject>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Delete", "Project", id,
                    "Project not found", ct);
                throw new Exception("Project not found");
            }

            xpo.Delete();

            await _logger.LogSuccessAsync(uow, "Delete", "Project", id, ct);

            return true;
        });
    }
}