using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoRoleMapper
{
    // ---------------------------------------------------------
    // DOMAIN → XPO
    // ---------------------------------------------------------
    public static XpoRole ToXpo(Role domain, UnitOfWork uow)
    {
        var xpo = uow.GetObjectByKey<XpoRole>(domain.Id)
                  ?? new XpoRole(uow)
                  {
                      Id = domain.Id
                  };

        xpo.Name = domain.Name;

        // ---------------------------------------------------------
        // SYNC PERMISSIONS
        // ---------------------------------------------------------

        // 1. Rimuovi permessi non più presenti nel Domain
        foreach (var existing in xpo.Permissions.ToList())
        {
            if (!domain.Permissions.Any(p => p.Id == existing.Id))
                xpo.Permissions.Remove(existing);
        }

        // 2. Aggiungi permessi mancanti
        foreach (var perm in domain.Permissions)
        {
            var xpoPerm = uow.GetObjectByKey<XpoPermission>(perm.Id)
                        ?? XpoPermissionMapper.ToXpo(perm, uow);

            if (!xpo.Permissions.Contains(xpoPerm))
                xpo.Permissions.Add(xpoPerm);
        }

        // ---------------------------------------------------------
        // SYNC USERS
        // ---------------------------------------------------------

        foreach (var existing in xpo.Users.ToList())
        {
            if (!domain.Users.Any(u => u.Id == existing.Id))
                xpo.Users.Remove(existing);
        }

        foreach (var user in domain.Users)
        {
            var xpoUser = uow.GetObjectByKey<XpoUser>(user.Id)
                        ?? XpoUserMapper.ToXpo(user, uow);

            if (!xpo.Users.Contains(xpoUser))
                xpo.Users.Add(xpoUser);
        }

        return xpo;
    }
    // ---------------------------------------------------------
    // XPO → DOMAIN
    // ---------------------------------------------------------
    public static Role ToDomain(XpoRole xpo)
    {
        var domain = new Role(
            id: xpo.Id,
            name: xpo.Name
        );

        foreach (var perm in xpo.Permissions)
            domain.Permissions.Add(XpoPermissionMapper.ToDomain(perm));

        return domain;
    }

    // ---------------------------------------------------------
    // DOMAIN → DTO
    // ---------------------------------------------------------
    public static RoleDto ToDto(Role domain)
    {
        return new RoleDto
        {
            Id = domain.Id,
            Name = domain.Name,
            Permissions = domain.Permissions
                .Select(XpoPermissionMapper.ToDto)
                .ToList()
        };
    }
}