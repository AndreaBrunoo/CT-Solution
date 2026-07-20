using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class EmployeeService : IEmployeeService
{
    private readonly XpoDataContext _ctx;
    private readonly IActionLogger _logger;

    public EmployeeService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(id, ct);
            if (xpo == null || xpo.IsDeleted) return null;

            var domain = XpoEmployeeMapper.ToDomain(xpo);

            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<EmployeeDto?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var user = await uow.GetObjectByKeyAsync<XpoUser>(userId, ct);
            if (user == null || user.IsDeleted)
                return null;

            // Navigo la collection User.Employees: più robusto del confronto e.User.Id per il translator di XPO
            user.Employees.Reload();
            var xpo = user.Employees.FirstOrDefault(e => e.DeletedAt == null);

            if (xpo == null) return null;

            var domain = XpoEmployeeMapper.ToDomain(xpo);

            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<EmployeeDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoEmployee>()
                .Where(x => x.DeletedAt == null)
                .ToListAsync(ct);

            return list
            .Select(xpo =>
            {
                var domain = XpoEmployeeMapper.ToDomain(xpo);

                return XpoEmployeeMapper.ToDto(domain);
            })
            .ToList();
        });
    }

    public async Task<IReadOnlyList<EmployeeDto>?> GetByProjectIdAsync(Guid projectId, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var project = await uow.GetObjectByKeyAsync<XpoProject>(projectId, ct);
            if (project == null || project.IsDeleted)
                return null;

            var workLogs = await uow.Query<XpoWorkLog>()
                .Where(w => w.DeletedAt == null && w.IdProject == projectId)
                .ToListAsync(ct);

            var employees = workLogs
                .Select(w => w.Employee)
                .Where(e => e != null && e.DeletedAt == null)
                .Distinct()
                .Select(xpo =>
                {
                    var domain = XpoEmployeeMapper.ToDomain(xpo);
                    return XpoEmployeeMapper.ToDto(domain);
                })
                .ToList();

            return employees;
        });
    }

    public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(dto.Id, ct);
            if (xpo == null || xpo.IsDeleted)
            {
                await _logger.LogFailureAsync("Update", "Employee", dto.Id,
                    "Employee not found", ct);
                throw new Exception("Employee not found");
            }

            var domain = XpoEmployeeMapper.ToDomain(xpo);

            domain.UserName = dto.UserName;

            XpoEmployeeMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Update", "Employee", domain.Id, ct);

            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("SoftDelete", "Employee", id,
                    "Employee not found", ct);
                throw new Exception("Employee not found");
            }

            if (!xpo.IsDeleted)
            {
                xpo.IsDeleted = true;
                xpo.DeletedAt = DateTime.UtcNow;
            }

            await _logger.LogSuccessAsync(uow, "SoftDelete", "Employee", id, ct);

            return true;
        });
    }

    public async Task RestoreAsync(Guid id, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(id, ct);
            if (xpo == null)
            {
                await _logger.LogFailureAsync("Restore", "Employee", id,
                    "Employee not found", ct);
                throw new Exception("Employee not found");
            }

            xpo.IsDeleted = false;
            xpo.DeletedAt = null;

            await _logger.LogSuccessAsync(uow, "Restore", "Employee", id, ct);

            return true;
        });
    }
}
