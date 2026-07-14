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
    private readonly IActionLogger _logger;

    public PermissionService(UnitOfWork uow, IActionLogger logger)
    {
        _ctx = new XpoDataContext(uow);
        _logger = logger;
    }

    // ---------------------------------------------------------
    // CREATE PERMISSION
    // ---------------------------------------------------------
    public async Task<PermissionDto>CreatePermissionAsync(CreatePermissionDto dto, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoPermission>()
                .FirstOrDefaultAsync(p => p.Code == dto.Code, ct);

            if (existing != null)
            {
                await _logger.LogFailureAsync("Create", "Permission", null,
                    $"Permission '{dto.Code}' already exists", ct);
                throw new Exception("Permission already exists");
            }

            var domain = new Permission(
                id: Guid.NewGuid(),
                code: dto.Code
            );

            var xpo = XpoPermissionMapper.ToXpo(domain, uow);

            await _logger.LogSuccessAsync(uow, "Create", "Permission", domain.Id, ct);

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
            var perm = await uow.GetObjectByKeyAsync<XpoPermission>(permissionId, ct);
            
            if (perm == null)
            {
                await _logger.LogFailureAsync("Delete", "Permission", permissionId,
                    "Permission not found", ct);
                throw new Exception("Permission not found");
            }

            perm.Delete();

            await _logger.LogSuccessAsync(uow, "Delete", "Permission", permissionId, ct);

            return true;
        });
    }
}