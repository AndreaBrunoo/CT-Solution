using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoRoleMapper
{
    public static XpoRole ToXpo(Role domain, UnitOfWork uow)
    {
        var xpo = uow.GetObjectByKey<XpoRole>(domain.Id)
                  ?? new XpoRole(uow)
                  {
                      Id = domain.Id
                  };

        xpo.Name = domain.Name;
        xpo.IsDeleted = domain.IsDeleted;
        xpo.DeletedAt = domain.DeletedAt;

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

    public static Role ToDomain(XpoRole xpo)
    {
        return new Role(
            id: xpo.Id,
            name: xpo.Name
        )
        {
            IsDeleted = xpo.IsDeleted,
            DeletedAt = xpo.DeletedAt
        };
    }

    public static RoleDto ToDto(Role domain)
    {
        return new RoleDto
        {
            Id = domain.Id,
            Name = domain.Name,
            IsDeleted = domain.IsDeleted,
            DeletedAt = domain.DeletedAt
        };
    }
}