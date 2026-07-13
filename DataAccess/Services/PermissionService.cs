using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.Interfaces;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;
using Sln.DataAccess.DataContext;
using Sln.DataAccess.Mappers;

namespace Sln.DataAccess.Services;

public class PermissionService : IPermissionService
{
    private readonly XpoDataContext _ctx;

    public PermissionService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    // ---------------------------------------------------------
    // CREATE PERMISSION
    // ---------------------------------------------------------
    public async Task<PermissionDto>CreatePermissionAsync(string code, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoPermission>()
                .FirstOrDefaultAsync(p => p.Code == code, ct);

            if (existing != null)
                throw new Exception("Permission already exists");

            var domain = new Permission(
                id: Guid.NewGuid(),
                code: code
            );

            var xpo = XpoPermissionMapper.ToXpo(domain, uow);

            return XpoPermissionMapper.ToDto(domain);
        });
    }

    // ---------------------------------------------------------
    // GET ALL PERMISSIONS
    // ---------------------------------------------------------
    public async Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken ct)
    {
        return await _ctx.DoAsync(async uow =>
        {
            var list = await uow.Query<XpoPermission>().ToListAsync(ct);

            return list
                .Select(xpo =>
                {
                    var domain = XpoPermissionMapper.ToDomain(xpo);

                    return XpoPermissionMapper.ToDto(domain);
                })
                .ToList();
        });
    }

    // ---------------------------------------------------------
    // DELETE PERMISSION
    // ---------------------------------------------------------
    public async Task DeletePermissionAsync(Guid permissionId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var perm = await uow.GetObjectByKeyAsync<XpoPermission>(permissionId, ct)
                ?? throw new Exception("Permission not found");

            perm.Delete();

            return true;
        });
    }
}