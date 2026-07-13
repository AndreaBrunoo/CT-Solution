using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly XpoDataContext _ctx;

    public RolePermissionService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    // ---------------------------------------------------------
    // ASSIGN PERMISSION TO ROLE
    // ---------------------------------------------------------
    public async Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct)
                ?? throw new Exception("Role not found");

            var perm = await uow.GetObjectByKeyAsync<XpoPermission>(permissionId, ct)
                ?? throw new Exception("Permission not found");

            if (!role.Permissions.Contains(perm))
                role.Permissions.Add(perm);

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
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct)
                ?? throw new Exception("Role not found");

            var perm = await uow.GetObjectByKeyAsync<XpoPermission>(permissionId, ct)
                ?? throw new Exception("Permission not found");

            if (role.Permissions.Contains(perm))
                role.Permissions.Remove(perm);

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