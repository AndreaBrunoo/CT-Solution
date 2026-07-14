using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class WorkLogService : IWorkLogService
{
    private readonly XpoDataContext _ctx;
    private readonly IActionLogger _logger;

    public WorkLogService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    public async Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoWorkLog>(id, ct);
            if (xpo == null) return null;

            var domain = XpoWorkLogMapper.ToDomain(xpo);

            return XpoWorkLogMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<WorkLogDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoWorkLog>().ToListAsync(ct);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoWorkLogMapper.ToDomain(xpo);

                return XpoWorkLogMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoWorkLog>()
                .FirstOrDefaultAsync(w =>
                    w.Description == dto.Description &&
                    w.Date == dto.Date &&
                    w.Employee.Id == dto.IdEmployee,
                    ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Worklog", null,
                    $"Worklog '{dto.Name}' already exists", ct);
                throw new Exception("Worklog already exists");
            }

            var domain = new WorkLog(
                id: Guid.NewGuid(),
                name: dto.Name,
                description: dto.Description,
                hoursCounter: dto.HoursCounter,
                date: dto.Date,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                idProject: dto.IdProject,
                idEmployee: dto.IdEmployee,
                idCategory: dto.IdCategory,
                idStatus: dto.IdStatus
            );

            var xpo = XpoWorkLogMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Worklog", domain.Id, ct);

            return XpoWorkLogMapper.ToDto(domain);
        });
    }

    public async Task<WorkLogDto?> UpdateAsync(UpdateWorkLogDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoWorkLog>(dto.Id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Update", "Worklog", dto.Id,
                    "Worklog not found", ct);
                throw new Exception("Worklog not found");
            }

            var domain = XpoWorkLogMapper.ToDomain(xpo);

            domain.Name = dto.Name;
            domain.Description = dto.Description;
            domain.HoursCounter = dto.HoursCounter;
            domain.Date = dto.Date;
            domain.IdProject = dto.IdProject;
            domain.IdEmployee = dto.IdEmployee;
            domain.IdCategory = dto.IdCategory;
            domain.IdStatus = dto.IdStatus;
            domain.UpdatedAt = DateTime.UtcNow;

            XpoWorkLogMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Worklog", domain.Id, ct);

            return XpoWorkLogMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoWorkLog>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Delete", "WorkLog", id,
                    "WorkLog not found", ct);
                throw new Exception("WorkLog not found");
            }

            xpo.Delete();

            await _logger.LogSuccessAsync(uow, "Delete", "WorkLog", id, ct);

            return true;
        });
    }
}