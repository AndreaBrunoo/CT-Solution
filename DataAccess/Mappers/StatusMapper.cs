using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
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
        xpo.Id = domain.Id;
        xpo.Name = domain.Name;

        return xpo;
    }

    public static Status ToDomain(XpoStatus xpo)
    {
        return new Status(
            id: xpo.Id,
            name: xpo.Name
        );
    }

    public static StatusDto ToDto(Status domain)
    {
        return new StatusDto
        {
            Id = domain.Id,
            Name = domain.Name
        };
    }
}