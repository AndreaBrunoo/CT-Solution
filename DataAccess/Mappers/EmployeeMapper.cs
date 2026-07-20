using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoEmployeeMapper
{
    public static XpoEmployee ToXpo(Employee domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoEmployee>(domain.Id)
                  ?? new XpoEmployee(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id = domain.Id;
        xpo.UserName = domain.UserName;
        xpo.IsDeleted = domain.IsDeleted;
        xpo.DeletedAt = domain.DeletedAt;

        // Relations (caricate tramite ID)
        xpo.User = uow.GetObjectByKey<XpoUser>(domain.IdUser);

        return xpo;
    }

    public static Employee ToDomain(XpoEmployee xpo)
    {
        var domain = new Employee(
            id: xpo.Id,
            userName: xpo.UserName
        )
        {
            IsDeleted = xpo.IsDeleted,
            DeletedAt = xpo.DeletedAt
        };

        domain.User = xpo.User == null
            ? null
            : XpoUserMapper.ToDomain(xpo.User);

        return domain;
    }

    public static EmployeeDto ToDto(Employee domain)
    {
        return new EmployeeDto
        {
            Id = domain.Id,
            UserName = domain.UserName,
            Email = domain.User.Email,
            IsDeleted = domain.IsDeleted,
            DeletedAt = domain.DeletedAt
        };
    }
}