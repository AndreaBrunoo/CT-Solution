using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.DataAccess.XpoEntities;
using Sln.Domain.DTOs;

namespace Sln.DataAccess.Mappers;

public static class XpoProjectMapper
{
    public static XpoProject ToXpo(Project domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoProject>(domain.Id)
                  ?? new XpoProject(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id = domain.Id;
        xpo.Name = domain.Name;
        xpo.HourlyRate = domain.HourlyRate;

        // Relations (caricate tramite ID)
        xpo.Company = uow.GetObjectByKey<XpoCompany>(domain.IdCompany);

        return xpo;
    }

    public static Project ToDomain(XpoProject xpo)
    {
        var domain = new Project(
            id: xpo.Id,
            name: xpo.Name,
            hourlyRate: xpo.HourlyRate,
            idCompany: xpo.Company?.Id ?? Guid.Empty
        );

        domain.Company = xpo.Company != null
            ? new Company(xpo.Company.Id, xpo.Company.Name, xpo.Company.Email, xpo.Company.HourlyRate)
            : null;

        return domain;
    }

    public static ProjectDto ToDto(Project domain)
    {
        return new ProjectDto
        {
            Id = domain.Id,
            Name = domain.Name,
            IdCompany = domain.IdCompany,
            HourlyRate = domain.HourlyRate,
            CompanyName = domain.Company?.Name
        };
    }
}