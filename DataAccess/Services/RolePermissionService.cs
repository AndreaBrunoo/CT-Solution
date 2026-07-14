using DevExpress.Xpo;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly XpoDataContext _ctx;
    private readonly IActionLogger _logger;

    public RolePermissionService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    // ---------------------------------------------------------
    // ASSIGN PERMISSION TO ROLE
    // ---------------------------------------------------------
    public async Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);
            if (role != null)
            {
                await _logger.LogFailureAsync("AssignPermission", "Role", null,
                    $"Role '{roleId}' Role not found", ct);
                throw new Exception("Role not found");
            }

            var perm = await uow.GetObjectByKeyAsync<XpoPermission>(permissionId, ct);
            if (perm != null)
            {
                await _logger.LogFailureAsync("AssignPermission", "Permission", null,
                    $"Permission '{permissionId}' Permission not found", ct);
                throw new Exception("Permission not found");
            }

            if (!role.Permissions.Contains(perm))
                role.Permissions.Add(perm);

            await _logger.LogSuccessAsync(uow, "AssignPermission", "Permission", permissionId, ct);

            return true;
        });
    }

    // ---------------------------------------------------------
    // REMOVE PERMISSION FROM ROLE
    // ---------------------------------------------------------
    public async Task RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);
            if (role != null)
            {
                await _logger.LogFailureAsync("RemoveRolePermission", "Role", null,
                    $"Role '{roleId}' Role not found", ct);
                throw new Exception("Role not found");
            }
            
            var perm = await uow.GetObjectByKeyAsync<XpoPermission>(permissionId, ct);
            if (perm != null)
            {
                await _logger.LogFailureAsync("RemoveRolePermission", "Permission", null,
                    $"Permission '{permissionId}' Permission not found", ct);
                throw new Exception("Permission not found");
            }

            if (role.Permissions.Contains(perm))
                role.Permissions.Remove(perm);

            await _logger.LogSuccessAsync(uow, "RemoveRolePermission", "Permmission", permissionId, ct);

            return true;
        });
    }

    // ---------------------------------------------------------
    // CHECK IF ROLE HAS PERMISSION
    // ---------------------------------------------------------
    public async Task<bool> HasPermissionAsync(Guid roleId, string permissionCode, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct);
            if (role == null) return false;

            return role.Permissions.Any(p => p.Code == permissionCode);
        });
    }

    // ---------------------------------------------------------
    // GET ALL PERMISSIONS OF ROLE
    // ---------------------------------------------------------
    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(Guid roleId, CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct)
                ?? throw new Exception("Role not found");

            return role.Permissions
            .Select(xpo =>
            {
                var domain = XpoPermissionMapper.ToDomain(xpo);

                return XpoPermissionMapper.ToDto(domain);
            })
            .ToList();
        });
    }
}