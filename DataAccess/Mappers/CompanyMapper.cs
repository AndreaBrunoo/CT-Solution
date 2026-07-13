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

        return xpo;
    }

    public static Company ToDomain(XpoCompany xpo)
    {
        return new Company(
            id: xpo.Id,
            name: xpo.Name,
            email: xpo.Email
        );
    }

    public static CompanyDto ToDto(Company domain)
    {
        return new CompanyDto
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email
        };
    }
}