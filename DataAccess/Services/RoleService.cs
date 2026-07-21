using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class RoleService : IRoleService
{
    private readonly XpoDataContext _ctx;
    private readonly IActionLogger _logger;

    public RoleService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    // ---------------------------------------------------------
    // CREATE ROLE
    // ---------------------------------------------------------
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoRole>()
                .FirstOrDefaultAsync(r => r.DeletedAt == null && r.Name == dto.Name, ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Role", null,
                    $"Role '{dto.Name}' already exists", ct);
                throw new Exception("Role already exists");
            }

            var domain = new Role(
                id: Guid.NewGuid(),
                name: dto.Name
            );

            var xpo = XpoRoleMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Role", domain.Id, ct);

            return XpoRoleMapper.ToDto(domain);
        });
    }

    // ---------------------------------------------------------
    // GET ALL ROLES
    // ---------------------------------------------------------
    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoRole>()
                .Where(x => x.DeletedAt == null)
                .ToListAsync(ct);

            return list
                .Select(xpo =>
                {
                    var domain = XpoRoleMapper.ToDomain(xpo);

                    return XpoRoleMapper.ToDto(domain);
                })
                .ToList();
        });
    }

    // ---------------------------------------------------------
    // DELETE ROLE (soft delete)
    // ---------------------------------------------------------
    public async Task DeleteRoleAsync(Guid roleId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);

            if (role == null)
            {
                await _logger.LogFailureAsync("SoftDelete", "Role", roleId,
                    "Role not found", ct);
                throw new Exception("Role not found");
            }

            if (!role.IsDeleted)
            {
                role.IsDeleted = true;
                role.DeletedAt = DateTime.UtcNow;
            }

            await _logger.LogSuccessAsync(uow, "SoftDelete", "Role", roleId, ct);

            return true;
        });
    }

    // ---------------------------------------------------------
    // RESTORE ROLE
    // ---------------------------------------------------------
    public async Task RestoreRoleAsync(Guid roleId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);

            if (role == null)
            {
                await _logger.LogFailureAsync("Restore", "Role", roleId,
                    "Role not found", ct);
                throw new Exception("Role not found");
            }

            role.IsDeleted = false;
            role.DeletedAt = null;

            await _logger.LogSuccessAsync(uow, "Restore", "Role", roleId, ct);

            return true;
        });
    }
}
