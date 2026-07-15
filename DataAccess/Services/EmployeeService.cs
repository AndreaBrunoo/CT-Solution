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
            if (xpo == null) return null;

            var domain = XpoEmployeeMapper.ToDomain(xpo);

            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<IReadOnlyList<EmployeeDto>?> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoEmployee>().ToListAsync(ct);

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
            if (project == null)
                return null;

            var workLogs = await uow.Query<XpoWorkLog>()
                .Where(w => w.IdProject == projectId)
                .ToListAsync(ct);

            var employees = workLogs
                .Select(w => w.Employee)
                .Where(e => e != null)
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

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoEmployee>()
                .FirstOrDefaultAsync(e =>
                e.UserName == dto.UserName &&
                e.User.Id == dto.IdUser,
                ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Employee", null,
                    $"Employee '{dto.UserName}' already exists", ct);
                throw new Exception("Employee already exists");
            }

            var domain = new Employee(
                id: Guid.NewGuid(),
                userName: dto.UserName,
                idUser: dto.IdUser
            );
            var xpo = XpoEmployeeMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Employee", domain.Id, ct);
            
            return XpoEmployeeMapper.ToDto(domain);
        });
    }

    public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var xpo = await uow.GetObjectByKeyAsync<XpoEmployee>(dto.Id, ct);
            if (xpo == null)
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
                await _logger.LogFailureAsync("Delete", "Employee", id,
                    "Employee not found", ct);
                throw new Exception("Employee not found");
            }

            xpo.Delete();

            await _logger.LogSuccessAsync(uow, "Delete", "Employee", id, ct);

            return true;
        });
    }
}