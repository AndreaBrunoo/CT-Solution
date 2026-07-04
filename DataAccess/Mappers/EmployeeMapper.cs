using DevExpress.Xpo;
using Sln.Domain.Entities;
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
        xpo.Id         = domain.Id;
        xpo.UserName   = domain.UserName;

        // Relations (caricate tramite ID)
        xpo.User   = uow.GetObjectByKey<XpoUser>(domain.IdUser);

        return xpo;
    }

    public static Employee ToDomain(XpoEmployee xpo)
    {
        return new Employee(
            id: xpo.Id,
            userName: xpo.UserName,
            idUser: xpo.User?.Id ?? Guid.Empty
        );
    }
}