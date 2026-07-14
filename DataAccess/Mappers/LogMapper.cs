using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoLogMapper
{
    public static LogDto ToDto(XpoLog xpo)
    {
        return new LogDto
        {
            Id = xpo.Id,
            Timestamp = xpo.Timestamp,
            Action = xpo.Action,
            Entity = xpo.Entity,
            EntityId = xpo.EntityId,
            UserId = xpo.UserId,
            UserEmail = xpo.UserEmail,
            Success = xpo.Success,
            ErrorMessage = xpo.ErrorMessage
        };
    }
}