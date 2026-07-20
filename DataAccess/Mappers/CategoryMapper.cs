using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoCategoryMapper
{
    public static XpoCategory ToXpo(Category domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoCategory>(domain.Id)
                  ?? new XpoCategory(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id            = domain.Id;
        xpo.Name          = domain.Name;
        xpo.IsDeleted     = domain.IsDeleted;
        xpo.DeletedAt     = domain.DeletedAt;

        return xpo;
    }

    public static Category ToDomain(XpoCategory xpo)
    {
        return new Category(
            id: xpo.Id,
            name: xpo.Name
        )
        {
            IsDeleted = xpo.IsDeleted,
            DeletedAt = xpo.DeletedAt
        };
    }

    public static CategoryDto ToDto(Category domain)
    {
        return new CategoryDto
        {
            Id = domain.Id,
            Name = domain.Name,
            IsDeleted = domain.IsDeleted,
            DeletedAt = domain.DeletedAt
        };
    }
}