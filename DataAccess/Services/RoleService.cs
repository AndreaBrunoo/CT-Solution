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

    public RoleService(UnitOfWork uow)
    {
        _ctx = new XpoDataContext(uow);
    }

    // ---------------------------------------------------------
    // CREATE ROLE
    // ---------------------------------------------------------
    public async Task<RoleDto> CreateRoleAsync(string name, CancellationToken ct)
    {
        return await _ctx.DoTranAsync(async uow =>
        {
            var existing = await uow.Query<XpoRole>()
                .FirstOrDefaultAsync(r => r.Name == name, ct);

            if (existing != null)
                throw new Exception("Role already exists");

            var domain = new Role(
                id: Guid.NewGuid(),
                name: name
            );

            var xpo = XpoRoleMapper.ToXpo(domain, uow);

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
            var list = await uow.Query<XpoRole>().ToListAsync(ct);

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
    // DELETE ROLE
    // ---------------------------------------------------------
    public async Task DeleteRoleAsync(Guid roleId, CancellationToken ct)
    {
        await _ctx.DoTranAsync(async uow =>
        {
            var role = await uow.GetObjectByKeyAsync<XpoRole>(roleId, ct)
                ?? throw new Exception("Role not found");

            role.Delete();

            return true;
        });
    }
}