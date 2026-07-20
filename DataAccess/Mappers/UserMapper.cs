using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoUserMapper
{
    public static XpoUser ToXpo(User domain, UnitOfWork uow)
    {
        var xpo = uow.GetObjectByKey<XpoUser>(domain.Id)
                  ?? new XpoUser(uow)
                  {
                      Id = domain.Id
                  };

        xpo.Email = domain.Email;
        xpo.PasswordHash = domain.PasswordHash;
        xpo.PasswordSalt = domain.PasswordSalt;
        xpo.IsDeleted = domain.IsDeleted;
        xpo.DeletedAt = domain.DeletedAt;

        xpo.Roles.Reload();

        var existingRoles = xpo.Roles.ToList();
        foreach (var r in existingRoles)
            xpo.Roles.Remove(r);

        foreach (var role in domain.Roles)
        {
            var xpoRole = uow.GetObjectByKey<XpoRole>(role.Id)
                         ?? new XpoRole(uow)
                         {
                             Id = role.Id,
                             Name = role.Name
                         };

            xpo.Roles.Add(xpoRole);
        }
        return xpo;
    }


    public static User ToDomain(XpoUser xpo)
    {
        xpo.Roles.Reload();
        xpo.Employees.Reload();

        var roles = xpo.Roles
            .Select(r => new Role(r.Id, r.Name))
            .ToList();

        var employee = xpo.Employees.FirstOrDefault() is { } xe
            ? new Employee(xe.Id, xe.UserName)
            : null;

        return new User(
            xpo.Id,
            xpo.Email,
            xpo.PasswordHash,
            xpo.PasswordSalt,
            roles
        )
        {
            Employee = employee,
            IsDeleted = xpo.IsDeleted,
            DeletedAt = xpo.DeletedAt
        };
    }

    public static UserDto ToDto(User domain)
    {
        return new UserDto
        {
            Id = domain.Id,
            Email = domain.Email,
            Roles = domain.Roles
            .Select(XpoRoleMapper.ToDto)
            .ToList(),
            IsDeleted = domain.IsDeleted,
            DeletedAt = domain.DeletedAt
        };
    }
}