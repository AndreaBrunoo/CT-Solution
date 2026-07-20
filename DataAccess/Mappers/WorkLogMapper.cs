using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoWorkLogMapper
{
    public static XpoWorkLog ToXpo(WorkLog domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoWorkLog>(domain.Id)
                  ?? new XpoWorkLog(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id = domain.Id;
        xpo.Name = domain.Name;
        xpo.Description = domain.Description;
        xpo.HoursCounter = domain.HoursCounter;
        xpo.Date = domain.Date;
        xpo.CreatedAt = domain.CreatedAt;
        xpo.UpdatedAt = domain.UpdatedAt;
        xpo.IsDeleted = domain.IsDeleted;
        xpo.DeletedAt = domain.DeletedAt;

        // Relations (caricate tramite ID)
        xpo.Project = uow.GetObjectByKey<XpoProject>(domain.IdProject);
        xpo.Employee = uow.GetObjectByKey<XpoEmployee>(domain.IdEmployee);
        xpo.Category = uow.GetObjectByKey<XpoCategory>(domain.IdCategory);
        xpo.Status = uow.GetObjectByKey<XpoStatus>(domain.IdStatus);

        return xpo;
    }

    public static WorkLog ToDomain(XpoWorkLog xpo)
    {
        return new WorkLog(
            id: xpo.Id,
            name: xpo.Name,
            description: xpo.Description,
            hoursCounter: xpo.HoursCounter,
            date: xpo.Date,
            createdAt: xpo.CreatedAt,
            updatedAt: xpo.UpdatedAt,
            idProject: xpo.Project?.Id ?? Guid.Empty,
            idEmployee: xpo.Employee?.Id ?? Guid.Empty,
            idCategory: xpo.Category?.Id ?? Guid.Empty,
            idStatus: xpo.Status?.Id ?? Guid.Empty
        )
        {
            IsDeleted = xpo.IsDeleted,
            DeletedAt = xpo.DeletedAt
        };
    }

    public static WorkLogDto ToDto(WorkLog domain, XpoWorkLog xpo)
    {
        return new WorkLogDto
        {
            Id = domain.Id,
            Name = domain.Name,
            Description = domain.Description,
            HoursCounter = domain.HoursCounter,
            Date = domain.Date,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            IdProject = domain.IdProject,
            IdEmployee = domain.IdEmployee,
            IdCategory = domain.IdCategory,
            IdStatus = domain.IdStatus,

            ProjectName = xpo.Project?.Name,
            EmployeeName = xpo.Employee?.UserName,
            CategoryName = xpo.Category?.Name,
            StatusName = xpo.Status?.Name,

            IsDeleted = domain.IsDeleted,
            DeletedAt = domain.DeletedAt
        };
    }
}