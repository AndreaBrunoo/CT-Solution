using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoPermissionMapper
{
    // ---------------------------------------------------------
    // DOMAIN → XPO
    // ---------------------------------------------------------
    public static XpoPermission ToXpo(Permission domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoPermission>(domain.Id)
                  ?? new XpoPermission(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Code = domain.Code;

        // ---------------------------------------------------------
        // SYNC ROLES (many-to-many)
        // ---------------------------------------------------------

        // 1. Rimuovi ruoli non più presenti nel Domain
        foreach (var existing in xpo.Roles.ToList())
        {
            if (!domain.Roles.Any(r => r.Id == existing.Id))
                xpo.Roles.Remove(existing);
        }

        // 2. Aggiungi ruoli mancanti
        foreach (var role in domain.Roles)
        {
            var xpoRole = uow.GetObjectByKey<XpoRole>(role.Id)
                        ?? XpoRoleMapper.ToXpo(role, uow);

            if (!xpo.Roles.Contains(xpoRole))
                xpo.Roles.Add(xpoRole);
        }

        return xpo;
    }

    // ---------------------------------------------------------
    // XPO → DOMAIN
    // ---------------------------------------------------------
    public static Permission ToDomain(XpoPermission xpo)
{
    return new Permission(
        id: xpo.Id,
        code: xpo.Code
    );
}

    // ---------------------------------------------------------
    // DOMAIN → DTO
    // ---------------------------------------------------------
    public static PermissionDto ToDto(Permission domain)
    {
        return new PermissionDto
        {
            Id = domain.Id,
            Code = domain.Code
        };
    }
}