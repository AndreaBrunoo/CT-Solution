using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoStatusMapper
{
    public static XpoStatus ToXpo(Status domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoStatus>(domain.Id) 
                  ?? new XpoStatus(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id            = domain.Id;
        xpo.Name   = domain.Name;

        return xpo;
    }

    public static Status ToDomain(XpoStatus xpo)
    {
        return new Status(
            id: xpo.Id,
            name: xpo.Name
        );
    }
}