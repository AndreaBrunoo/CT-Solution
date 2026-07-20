using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.DataAccess.XpoEntities;
using Sln.Domain.DTOs;

namespace Sln.DataAccess.Mappers;

public static class XpoCompanyMapper
{
    public static XpoCompany ToXpo(Company domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoCompany>(domain.Id)
                  ?? new XpoCompany(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id            = domain.Id;
        xpo.Name          = domain.Name;
        xpo.Email         = domain.Email;
        xpo.IsDeleted     = domain.IsDeleted;
        xpo.DeletedAt     = domain.DeletedAt;

        return xpo;
    }

    public static Company ToDomain(XpoCompany xpo)
    {
        return new Company(
            id: xpo.Id,
            name: xpo.Name,
            email: xpo.Email
        )
        {
            IsDeleted = xpo.IsDeleted,
            DeletedAt = xpo.DeletedAt
        };
    }

    public static CompanyDto ToDto(Company domain)
    {
        return new CompanyDto
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email,
            IsDeleted = domain.IsDeleted,
            DeletedAt = domain.DeletedAt
        };
    }
}