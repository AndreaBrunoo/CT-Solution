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
            if (xpo == null || xpo.IsDeleted) return null;

            var domain = XpoWorkLogMapper.ToDomain(xpo);

            return XpoWorkLogMapper.ToDto(domain, xpo);
        });
    }

    public async Task<IReadOnlyList<WorkLogDto>> GetMineAsync(Guid userId, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            // 1. Trovo lo User dal token
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (user == null || user.IsDeleted)
                return new List<WorkLogDto>();

            // 2. Trovo l'Employee associato allo User navigando la collection User.Employees
            //    (più robusto per il translator di XPO rispetto a confrontare e.User.Id)
            user.Employees.Reload();
            var employee = user.Employees.FirstOrDefault(e => !e.IsDeleted);

            if (employee == null)
                return new List<WorkLogDto>();

            // 3. Trovo i WorkLog dell'Employee (solo non soft-deleted).
            //    Importante: estraggo l'Id in una variabile locale prima della lambda
            //    perché il translator LINQ di XPO non riesce a serializzare l'accesso
            //    a una proprietà di un'altra entità XPO (employee.Id) all'interno
            //    dell'expression tree: genera un ExpressionAccessOperator non traducibile.
            var employeeId = employee.Id;
            var worklogs = await uow.Query<XpoWorkLog>()
                .Where(w => w.DeletedAt == null && w.IdEmployee == employeeId)
                .ToListAsync(ct);

            // 4. Mappo anche le navigation
            return worklogs
                .Select(xpo =>
                {
                    var domain = XpoWorkLogMapper.ToDomain(xpo);
                    return XpoWorkLogMapper.ToDto(domain, xpo);
                })
                .ToList();
        });
    }

    public async Task<IReadOnlyList<WorkLogDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoWorkLog>()
                .Where(x => x.DeletedAt == null)
                .ToListAsync(ct);

            if (list == null) return null;
            return list
            .Select(xpo =>
            {
                var domain = XpoWorkLogMapper.ToDomain(xpo);

                return XpoWorkLogMapper.ToDto(domain, xpo);
            })
            .ToList();
        });
    }

    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            // Escludo i WL già soft-deleted dal check di unicità
            var existing = await uow.Query<XpoWorkLog>()
                .FirstOrDefaultAsync(w =>
                    w.DeletedAt == null &&
                    w.Name == dto.Name &&
                    w.Date == dto.Date &&
                    w.IdEmployee == dto.IdEmployee,
                    ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Worklog", null,
                    $"Worklog '{dto.Name}' already exists", ct);
                throw new Exception("Worklog already exists");
            }

            // Validazione entità collegate: escludo soft-deleted
            var project = await uow.GetObjectByKeyAsync<XpoProject>(dto.IdProject, ct);
            if (project == null || project.IsDeleted)
            {
                await _logger.LogFailureAsync("Create", "Worklog", null,
                    $"Project '{dto.IdProject}' not found or deleted", ct);
                throw new Exception("Project not found or deleted");
            }

            var employee = await uow.GetObjectByKeyAsync<XpoEmployee>(dto.IdEmployee, ct);
            if (employee == null || employee.IsDeleted)
            {
                await _logger.LogFailureAsync("Create", "Worklog", null,
                    $"Employee '{dto.IdEmployee}' not found or deleted", ct);
                throw new Exception("Employee not found or deleted");
            }

            var category = await uow.GetObjectByKeyAsync<XpoCategory>(dto.IdCategory, ct);
            if (category == null || category.IsDeleted)
            {
                await _logger.LogFailureAsync("Create", "Worklog", null,
                    $"Category '{dto.IdCategory}' not found or deleted", ct);
                throw new Exception("Category not found or deleted");
            }

            var status = await uow.GetObjectByKeyAsync<XpoStatus>(dto.IdStatus, ct);
            if (status == null || status.IsDeleted)
            {
                await _logger.LogFailureAsync("Create", "Worklog", null,
                    $"Status '{dto.IdStatus}' not found or deleted", ct);
                throw new Exception("Status not found or deleted");
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

            return XpoWorkLogMapper.ToDto(domain, xpo);
        });
    }

    public async Task<WorkLogDto?> UpdateAsync(UpdateWorkLogDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoWorkLog>(dto.Id, ct);
            if (xpo == null || xpo.IsDeleted)
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
            domain.UpdatedAt = DateTime.UtcNow;

            XpoWorkLogMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Worklog", domain.Id, ct);

            return XpoWorkLogMapper.ToDto(domain, xpo);
        });
    }

    public async Task<WorkLogDto> ChangeStatusAsync(Guid worklogId, Guid newStatusId, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpoWorkLog = await uow.GetObjectByKeyAsync<XpoWorkLog>(worklogId, ct);
            if (xpoWorkLog == null || xpoWorkLog.IsDeleted)
            {
                await _logger.LogFailureAsync("ChangeStatus", "Worklog", worklogId,
                    "Worklog not found", ct);
                throw new Exception("Worklog not found");
            }

            var xpoStatus = await uow.GetObjectByKeyAsync<XpoStatus>(newStatusId, ct);
            if (xpoStatus == null || xpoStatus.IsDeleted)
            {
                await _logger.LogFailureAsync("ChangeStatus", "Status", newStatusId,
                    "Status not found", ct);
                throw new Exception("Status not found");
            }
            ;

            var domain = XpoWorkLogMapper.ToDomain(xpoWorkLog);

            domain.IdStatus = newStatusId;

            var xpo = XpoWorkLogMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "ChangeStatus", "WorkLog", worklogId, ct);

            return XpoWorkLogMapper.ToDto(domain, xpo);
        });
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoWorkLog>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("SoftDelete", "WorkLog", id,
                    "WorkLog not found", ct);
                throw new Exception("WorkLog not found");
            }

            if (!xpo.IsDeleted)
            {
                xpo.IsDeleted = true;
                xpo.DeletedAt = DateTime.UtcNow;
            }

            await _logger.LogSuccessAsync(uow, "SoftDelete", "WorkLog", id, ct);

            return true;
        });
    }

    public async Task RestoreAsync(Guid id, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoWorkLog>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Restore", "WorkLog", id,
                    "WorkLog not found", ct);
                throw new Exception("WorkLog not found");
            }

            xpo.IsDeleted = false;
            xpo.DeletedAt = null;

            await _logger.LogSuccessAsync(uow, "Restore", "WorkLog", id, ct);

            return true;
        });
    }
}
